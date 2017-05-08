namespace CashCenter.DataMigration.Providers.Dbf.Entities
{
    public class DbfWaterCustomer
    {
        public int Number { get; private set; }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public int CounterNumber1 { get; private set; }
        public int CounterNumber2 { get; private set; }
        public int CounterNumber3 { get; private set; }

        public DbfWaterCustomer(int number, string name, string address,
            int counterNumber1, int counterNumber2, int counterNumber3)
        {
            Number = number;
            Name = name;
            Address = address;
            CounterNumber1 = counterNumber1;
            CounterNumber2 = counterNumber2;
            CounterNumber3 = counterNumber3;
        }
    }
}
