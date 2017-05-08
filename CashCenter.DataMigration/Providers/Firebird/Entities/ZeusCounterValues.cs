namespace CashCenter.DataMigration.Providers.Firebird.Entities
{
    public class ZeusCounterValues
    {
        public int Id { get; private set; }
        public int CustomerNumber { get; private set; }
        public int CustomerCounterId { get; private set; }
        public int Value1 { get; private set; }
        public int? Value2 { get; private set; }

        public ZeusCounterValues(int id, int customerNumber,
            int customerCounterId, int value1, int? value2)
        {
            Id = id;
            CustomerNumber = customerNumber;
            CustomerCounterId = customerCounterId;
            Value1 = value1;
            Value2 = value2;
        }

        public ZeusCounterValues(int customerId, int customerCounterId, int value1, int? value2)
            : this(-1, customerId, customerCounterId, value1, value2)
        {
        }
    }
}
