using System;
using System.Text;

namespace CashCenter.Common.DataEntities
{
    public class CustomerPayment
    {
        public Customer Customer { get; private set; }
        public int NewDayValue { get; private set; }
        public int NewNightValue { get; private set; }
        public decimal Cost { get; private set; }
        public int KindId { get; private set; }
        public int ReasonId { get; private set; }
        public DateTime CreateDate { get; private set; }
        public string Description { get; private set; }

        public bool IsValid { get; private set; }
        public string ValidationErrorMessage { get; private set; }

        public CustomerPayment(Customer customer, int newDayValue, int newNightValue,
            decimal cost, int kindId, int reasonId, DateTime createDate)
        {
            Customer = customer;
            NewDayValue = newDayValue;
            NewNightValue = newNightValue;
            Cost = cost;
            KindId = kindId;
            ReasonId = reasonId;
            CreateDate = createDate;

            ValidationErrorMessage = Validate();
            IsValid = string.IsNullOrEmpty(ValidationErrorMessage);
        }

        private string ValidateCost(decimal costValue)
        {
            if (costValue <= 0)
                return $"Сумма платежа должна быть положительна ({costValue}).";

            return null;
        }

        private string ValidatePaymentKindId(int paymentKindId)
        {
            if (paymentKindId < 0)
                return $"Вид платежа некорректен (id = {paymentKindId}).";

            return null;
        }

        private string ValidateDayCounterValue(int dayCounterValue)
        {
            if (Customer == null)
                return "Плательщик не задан";

            if (Customer.IsNormative)
                return null;

            if (dayCounterValue < Customer.DayValue)
                return $"Новое показание дневного счетчика меньше предыдущего ({dayCounterValue} < {Customer.DayValue}).";

            return null;
        }

        private string ValidateNightCounterValue(int nightCounterValue)
        {
            if (Customer == null)
                return "Плательщик не задан";

            if (Customer.IsNormative)
                return null;

            if (!Customer.IsTwoTariff)
                return null;

            if (nightCounterValue < Customer.NightValue)
                return $"Новое показание ночного счетчика меньше меньше предыдущего ({nightCounterValue} < {Customer.NightValue}).";

            return null;
        }

        private string ValidateReasonId(int reasonId)
        {
            if (reasonId < 0)
                return $"Основание платежа некорректно (id = {reasonId}).";

            return null;
        }

        private string Validate()
        {
            var validators = new Func<string>[]
            {
                () => ValidateCost(Cost),
                () => ValidatePaymentKindId(KindId),
                () => ValidateDayCounterValue(NewDayValue),
                () => ValidateNightCounterValue(NewNightValue),
                () => ValidateReasonId(ReasonId)
            };

            var sbResultError = new StringBuilder();
            foreach (var validator in validators)
            {
                var validatorResult = validator();
                if (validatorResult != null)
                    sbResultError.AppendLine(validatorResult);
            }

            return sbResultError.ToString();
        }
    }
}