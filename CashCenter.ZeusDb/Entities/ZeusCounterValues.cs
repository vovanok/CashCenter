namespace CashCenter.ZeusDb.Entities
{
    public class ZeusCounterValues
    {
        public int Id { get; private set; }

        public int CustomerId { get; private set; }

        public int CustomerCounterId { get; private set; }

        public int Value1 { get; private set; }

        public int? Value2 { get; private set; }

        public ZeusCounterValues(int id, int customerId,
            int customerCounterId, int value1, int? value2)
        {
            Id = id;
            CustomerId = customerId;
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
