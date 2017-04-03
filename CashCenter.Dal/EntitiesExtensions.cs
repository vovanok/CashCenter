using System;
using System.Text;

namespace CashCenter.Dal
{
    public static class EntitiesExtensions
    {
        public static bool IsNormative(this Customer customer)
        {
            return customer.DayValue <= 0 && customer.NightValue <= 0;
        }

        public static bool IsTwoTariff(this Customer customer)
        {
            return customer.DayValue > 0 && customer.NightValue > 0;
        }

        public static bool IsValid(this CustomerPayment customerPayment, Customer customer, out string errorMessage)
        {
            var validators = new Func<string>[]
            {
                () => customerPayment.ValidateCost(customerPayment.Cost),
                () => customerPayment.ValidateDayCounterValue(customer, customerPayment.NewDayValue),
                () => customerPayment.ValidateNightCounterValue(customer, customerPayment.NewNightValue),
                () => customerPayment.ValidateReasonId(customerPayment.ReasonId)
            };

            var sbResultError = new StringBuilder();
            foreach (var validator in validators)
            {
                var validatorResult = validator();
                if (validatorResult != null)
                    sbResultError.AppendLine(validatorResult);
            }

            errorMessage = sbResultError.ToString();
            return string.IsNullOrEmpty(errorMessage);
        }

        private static string ValidateCost(this CustomerPayment customerPayment, decimal costValue)
        {
            if (costValue <= 0)
                return $"Сумма платежа должна быть положительна ({costValue}).";

            return null;
        }

        private static string ValidateDayCounterValue(this CustomerPayment customerPayment, Customer customer, int dayCounterValue)
        {
            if (customer == null)
                return "Плательщик не задан";

            if (customer.IsNormative())
                return null;

            if (dayCounterValue < customer.DayValue)
                return $"Новое показание дневного счетчика меньше предыдущего ({dayCounterValue} < {customer.DayValue}).";

            return null;
        }

        private static string ValidateNightCounterValue(this CustomerPayment customerPayment, Customer customer, int nightCounterValue)
        {
            if (customer == null)
                return "Плательщик не задан";

            if (customer.IsNormative())
                return null;

            if (!customer.IsTwoTariff())
                return null;

            if (nightCounterValue < customer.NightValue)
                return $"Новое показание ночного счетчика меньше меньше предыдущего ({nightCounterValue} < {customer.NightValue}).";

            return null;
        }

        private static string ValidateReasonId(this CustomerPayment customerPayment, int reasonId)
        {
            if (reasonId < 0)
                return $"Основание платежа некорректно (id = {reasonId}).";

            return null;
        }
    }
}
