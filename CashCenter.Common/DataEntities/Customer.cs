namespace CashCenter.Common.DataEntities
{
    public class Customer
    {
        public int Id { get; private set; }
        public string DepartmentCode { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public int DayValue { get; private set; }
        public int NightValue { get; private set; }
        public decimal Balance { get; private set; }
        public decimal Penalty { get; private set; }

        public bool IsNormative => DayValue <= 0 && NightValue <= 0;
        public bool IsTwoTariff => DayValue > 0 && NightValue > 0;

        public Customer(int id, string departmentCode, string name, string address,
            int dayValue, int nightValue, decimal balance, decimal penalty)
        {
            Id = id;
            DepartmentCode = departmentCode;
            Name = name;
            Address = address;
            DayValue = dayValue;
            NightValue = nightValue;
            Balance = balance;
            Penalty = penalty;
        }
    }
}
