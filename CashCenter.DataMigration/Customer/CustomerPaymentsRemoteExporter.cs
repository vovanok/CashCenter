using CashCenter.Dal;
using CashCenter.ZeusDb;
using CashCenter.ZeusDb.Entities;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CashCenter.DataMigration
{
    public class CustomerPaymentsRemoteExporter : BaseExporter<CustomerPayment>
    {
        private const string PAY_JOURNAL_NAME = "Пачка квитанций от РКЦ Ивановской области";
        private const int PAYMENT_KIND_ID = 123;
        private const string PAYMENT_KIND_NAME = "РКЦ Ивановской области";
        private const int PAYMENT_KIND_TYPE_ID = 1;

        protected override List<CustomerPayment> GetSourceItems(DateTime beginDatetime, DateTime endDatetime)
        {
            return DalController.Instance.CustomerPayments.Where(customerPayment =>
                 beginDatetime <= customerPayment.CreateDate && customerPayment.CreateDate <= endDatetime).ToList();
        }

        protected override int TryExportItems(IEnumerable<CustomerPayment> items)
        {
            var countSuccess = 0;
            foreach (var item in items)
            {
                if (TryExportOneItem(item))
                    countSuccess++;
            }

            return countSuccess;
        }

        private bool TryExportOneItem(CustomerPayment customerPayment)
        {
            if (customerPayment == null || customerPayment.Customer == null || customerPayment.Customer.Department == null)
                return false;

            try
            {
                var db = new ZeusDbController(
                    customerPayment.Customer.Department.Code,
                    customerPayment.Customer.Department.Url,
                    customerPayment.Customer.Department.Path);

                var existingPaymentKind = db.GetPaymentKind(PAYMENT_KIND_ID);
                if (existingPaymentKind == null)
                    db.AddPaymentKind(new ZeusPaymentKind(PAYMENT_KIND_ID, PAYMENT_KIND_NAME, PAYMENT_KIND_TYPE_ID));

                var payJournal = AddOrUpdatePayJournal(db, PAYMENT_KIND_ID, customerPayment.CreateDate, customerPayment.Cost);

                var customerCounterId = db.GetCustomerCounterId(customerPayment.Customer.Number);

                int? metersId = null;
                ZeusCounterValues counterValues = null;

                if (!customerPayment.Customer.IsNormative())
                {
                    int? correctedNightValue = customerPayment.Customer.IsTwoTariff() ? (int?)customerPayment.NewNightValue : 0;

                    // 1.
                    var newCounterValues = new ZeusCounterValues(customerPayment.Customer.Number, customerCounterId, customerPayment.NewDayValue, correctedNightValue);
                    counterValues = db.AddCounterValues(newCounterValues, customerPayment.CreateDate);

                    // 2.
                    var newMeters = new ZeusMeter(
                        -1,
                        customerPayment.Customer.Number,
                        customerCounterId,
                        customerPayment.NewDayValue,
                        correctedNightValue,
                        counterValues.Id);

                    var meters = db.AddMeters(newMeters);
                    metersId = meters.Id;
                }

                var penaltyTotal = GetPenaltyTotal(customerPayment.Customer.Balance, customerPayment.Customer.Penalty, customerPayment.Cost);

                // 3.
                var pay = db.AddPay(
                    new ZeusPay(
                        customerPayment.Customer.Number,
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
            catch
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