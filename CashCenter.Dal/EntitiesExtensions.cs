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
    }
}
