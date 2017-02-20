using System;
using CashCenter.Common.DataEntities;
using CashCenter.Common.DbQualification;
using CashCenter.Common;
using System.Linq;
using CashCenter.ZeusDb;
using System.Collections.Generic;
using CashCenter.ZeusDb.Entities;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    class OnlineCustomerSalesContext : BaseCustomerSalesContext
    {
        private const string PAY_JOURNAL_NAME = "Пачка квитанций от РКЦ Ивановской области";
        private const int PAYMENT_KIND_ID = 2000;
        private const string PAYMENT_KIND_NAME = "РКЦ Ивановской области";
        private const int PAYMENT_KIND_TYPE_ID = 2;

        private ZeusDbController db;
        private RegionDef regionDef;

        public OnlineCustomerSalesContext(int customerId, RegionDef regionDef, string departamentCode)
            : base(customerId)
        {
            this.regionDef = regionDef;
            var dbControllers = GetDbControllersByDepartamentCode(departamentCode);
            foreach (var dbController in dbControllers)
            {
                // Получение плательщика
                var zeusCustomer = dbController.GetCustomer(customerId);
                if (zeusCustomer != null)
                {
                    db = dbController;

                    var now = DateTime.Now;
                    var beginDate = new DateTime(now.Year, now.Month, 1);
                    var endDate = beginDate.AddMonths(1).AddDays(-1);

                    // Получение показаний счетчиков плательщика
                    var zeusCustomerCountersValues = db.GetCustomerCounterValues(customerId, beginDate, endDate);

                    // Получение задолженности плательщика
                    var zeusDebt = db.GetDebt(customerId, now.Year * 12 + now.Month);

                    Customer = new Common.DataEntities.Customer(
                        zeusCustomer.Id,
                        departamentCode ?? string.Empty,
                        zeusCustomer.Name,
                        GetZeusCustomerAddress(zeusCustomer),
                        zeusCustomerCountersValues != null ? zeusCustomerCountersValues.EndDayValue : 0,
                        zeusCustomerCountersValues != null ? zeusCustomerCountersValues.EndNightValue : 0,
                        zeusDebt != null ? zeusDebt.Balance : 0,
                        zeusDebt != null ? zeusDebt.Penalty : 0);

                    PaymentReasons = db.GetPaymentReasons()?
                            .Select(zeusReason => new Common.DataEntities.PaymentReason(zeusReason.Id, zeusReason.Name))?.ToList()
                        ?? new List<Common.DataEntities.PaymentReason>();
                    break;
                }
            }
        }

        public override bool Pay(CustomerPayment payment)
        {
            try
            {
                if (!IsCustomerFinded)
                {
                    Log.Error($"{ERROR_PREFIX}\nОтсутствует плательщик.");
                    return false;
                }

                if (payment == null)
                {
                    Log.Error($"{ERROR_PREFIX}\nОтсутствует платеж.");
                    return false;
                }

                if (!payment.IsValid)
                {
                    Log.Error($"{ERROR_PREFIX}\n{payment.ValidationErrorMessage}");
                    return false;
                }

                var existingPaymentKind = db.GetPaymentKind(PAYMENT_KIND_ID);
                if (existingPaymentKind == null)
                {
                    db.AddPaymentKind(new PaymentKind(PAYMENT_KIND_ID, PAYMENT_KIND_NAME, PAYMENT_KIND_TYPE_ID));
                }

                var payJournal = AddOrUpdatePayJournal(PAYMENT_KIND_ID, payment.CreateDate, payment.Cost);

                var customerCounterId = db.GetCustomerCounterId(Customer.Id);

                int? metersId = null;
                CounterValues counterValues = null;
                if (!Customer.IsNormative)
                {
                    int? correctedValue2 = Customer.IsTwoTariff ? (int?)payment.NewNightValue : null;

                    // 1.
                    counterValues = db.AddCounterValues(new CounterValues(Customer.Id, customerCounterId, payment.NewDayValue, correctedValue2), payment.CreateDate);

                    // 2.
                    var meters = db.AddMeters(new Meter(-1, Customer.Id, customerCounterId, payment.NewDayValue, correctedValue2, counterValues.Id));
                    metersId = meters.Id;
                }

                var penaltyTotal = GetPenaltyTotal(Customer.Balance, Customer.Penalty, payment.Cost);

                // 3.
                var pay = db.AddPay(new Pay(Customer.Id, payment.ReasonId, metersId, payJournal.Id, payment.Cost, penaltyTotal, payment.Description));

                // 4.
                if (counterValues != null)
                    db.UpdateCounterValuesPayId(counterValues.Id, pay.Id);

                var paymentReasonName = PaymentReasons.FirstOrDefault(item => item.Id == payment.ReasonId)?.Name ?? string.Empty;
                InfoForCheck = new InfoForCheck(payment.Cost, payment.CreateDate, Customer.DepartmentCode, Customer.Id, Customer.Name, paymentReasonName);

                return true;
            }
            catch(Exception e)
            {
                Log.ErrorWithException(ERROR_PREFIX, e);
                return false;
            }
        }

        private PayJournal AddOrUpdatePayJournal(int paymentKindId, DateTime createDate, decimal cost)
        {
            if (db == null)
            {
                Log.Error("Ошибка добавления/обновления журнала платежа. БД не задана.");
                return null;
            }

            var payJournal = db.GetPayJournal(createDate, paymentKindId);
            if (payJournal != null)
                db.AddRequireToPayJournal(payJournal, cost);
            else
                payJournal = db.AddPayJournal(new PayJournal(PAY_JOURNAL_NAME, createDate, paymentKindId), cost);

            return payJournal;
        }

        private List<ZeusDbController> GetDbControllersByDepartamentCode(string departamentCode)
        {
            return regionDef.Departments.Select(departmentDef => new ZeusDbController(departmentDef))
                .Where(dbController => dbController.DepartamentDef.Code == departamentCode).ToList();
        }

        private decimal GetPenaltyTotal(decimal balance, decimal penalty, decimal cost)
        {
            if (cost > balance && penalty > 0)
                return Math.Min(cost - balance, penalty);

            return 0;
        }

        private string GetZeusCustomerAddress(ZeusDb.Entities.Customer zeusCustomer)
        {
            if (zeusCustomer == null)
                return string.Empty;

            var addressComponents = new[]
            {
                zeusCustomer.LocalityName,
                zeusCustomer.StreetName,
                zeusCustomer.BuildingNumber,
                zeusCustomer.Flat
            };

            return string.Join(", ", addressComponents.Where(item => !string.IsNullOrEmpty(item)));
        }
    }
}
