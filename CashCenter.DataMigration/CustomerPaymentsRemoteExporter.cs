using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.ZeusDb;
using CashCenter.ZeusDb.Entities;
using System;
using System.Linq;

namespace CashCenter.DataMigration
{
    public class CustomerPaymentsRemoteExporter : BaseExporter
    {
        private const string PAY_JOURNAL_NAME = "Пачка квитанций от РКЦ Ивановской области";

        public override void Export(DateTime beginDatetime, DateTime endDatetime)
        {
            var customerPaymentsForExport = DalController.Instance.CustomerPayments.Where(customerPayment =>
                beginDatetime <= customerPayment.CreateDate && customerPayment.CreateDate <= endDatetime);

            var countSuccess = 0;
            foreach (var customerPayment in customerPaymentsForExport)
            {
                if (ExportCustomerPayment(customerPayment))
                    countSuccess++;
            }

            Log.Info($"Экспортировано {countSuccess} из {customerPaymentsForExport.Count()}");
        }

        private bool ExportCustomerPayment(CustomerPayment customerPayment)
        {
            if (customerPayment == null || customerPayment.Customer == null || customerPayment.Customer.Department == null)
                return false;

            try
            {
                var db = new ZeusDbController(
                    customerPayment.Customer.Department.Code,
                    customerPayment.Customer.Department.Url,
                    customerPayment.Customer.Department.Path);

                var existingPaymentKind = db.GetPaymentKind(customerPayment.PaymentKind.Id);
                if (existingPaymentKind == null)
                {
                    db.AddPaymentKind(new ZeusPaymentKind(
                        customerPayment.PaymentKind.Id,
                        customerPayment.PaymentKind.Name,
                        customerPayment.PaymentKind.TypeZeusId));
                }

                var payJournal = AddOrUpdatePayJournal(db, customerPayment.PaymentKind.Id, customerPayment.CreateDate, customerPayment.Cost);

                var customerCounterId = db.GetCustomerCounterId(customerPayment.Customer.Id);

                int? metersId = null;
                ZeusCounterValues counterValues = null;

                var isNormative = customerPayment.Customer.DayValue <= 0 && customerPayment.Customer.NightValue <= 0;
                var isTwoTariff = customerPayment.Customer.DayValue > 0 && customerPayment.Customer.NightValue > 0;
                if (!isNormative)
                {
                    int? correctedNightValue = isTwoTariff ? (int?)customerPayment.NewNightValue : null;

                    // 1.
                    counterValues = db.AddCounterValues(
                        new ZeusCounterValues(customerPayment.Customer.Id, customerCounterId, customerPayment.NewDayValue, correctedNightValue),
                        customerPayment.CreateDate);

                    // 2.
                    var meters = db.AddMeters(
                        new ZeusMeter(-1, customerPayment.Customer.Id, customerCounterId, customerPayment.NewDayValue, correctedNightValue, counterValues.Id));
                    metersId = meters.Id;
                }

                var penaltyTotal = GetPenaltyTotal(customerPayment.Customer.Balance, customerPayment.Customer.Penalty, customerPayment.Cost);

                // 3.
                var pay = db.AddPay(
                    new ZeusPay(
                        customerPayment.Customer.Id,
                        customerPayment.PaymentReason.Id,
                        metersId,
                        payJournal.Id,
                        customerPayment.Cost,
                        penaltyTotal,
                        customerPayment.Description));

                // 4.
                if (counterValues != null)
                    db.UpdateCounterValuesPayId(counterValues.Id, pay.Id);

                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private ZeusPayJournal AddOrUpdatePayJournal(ZeusDbController db, int paymentKindId, DateTime createDate, decimal cost)
        {
            if (db == null)
                return null;

            var payJournal = db.GetPayJournal(createDate, paymentKindId);
            if (payJournal != null)
                db.AddRequireToPayJournal(payJournal, cost);
            else
                payJournal = db.AddPayJournal(new ZeusPayJournal(PAY_JOURNAL_NAME, createDate, paymentKindId), cost);

            return payJournal;
        }

        private decimal GetPenaltyTotal(decimal balance, decimal penalty, decimal cost)
        {
            if (cost > balance && penalty > 0)
                return Math.Min(cost - balance, penalty);

            return 0;
        }
    }
}