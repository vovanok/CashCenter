namespace CashCenter.DataMigration.Providers.Firebird.Entities
{
    public class ZeusMeter
    {
        public int Id { get; private set; }

        public int CustomerNumber { get; private set; }

        public int CustomerCounterId { get; private set; }

        public int Value1 { get; private set; }

        public int? Value2 { get; private set; }

        public int CounterValuesId { get; private set; }

        public ZeusMeter(
            int id, int customerNumber,
            int customerCounterId, int value1,
            int? value2, int counterValuesId)
        {
            Id = id;
            CustomerNumber = customerNumber;
            CustomerCounterId = customerCounterId;
            Value1 = value1;
            Value2 = value2;
            CounterValuesId = counterValuesId;
        }
    }
}
