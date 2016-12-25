namespace CashCenter.IvEnergySales.DataModel
{
    public class Meter
    {
        public int Id { get; private set; }

        public int CustomerId { get; private set; }

        public int CustomerCounterId { get; private set; }

        public int Value1 { get; private set; }

        public int? Value2 { get; private set; }

        public int CounterValuesId { get; private set; }

        public Meter(int id, int customerId, int customerCounterId, int value1,
            int? value2, int counterValuesId)
        {
            Id = id;
            CustomerId = customerId;
            CustomerCounterId = customerCounterId;
            Value1 = value1;
            Value2 = value2;
            CounterValuesId = counterValuesId;
        }
    }
}
