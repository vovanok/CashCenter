using CashCenter.DataMigration.Dbf;

namespace CashCenter.DataMigration.Providers.Dbf.Entities
{
    public class DbfWaterCustomer
    {
        [NumericDbfColumn("SCHET")]
        public int Number { get; private set; }

        [CharacterDbfColumn("FIO")]
        public string Name { get; private set; }

        [CharacterDbfColumn("ADRESS")]
        public string Address { get; private set; }

        [CharacterDbfColumn("N_SHET1")]
        public string CounterNumber1 { get; private set; }

        [CharacterDbfColumn("N_SHET2")]
        public string CounterNumber2 { get; private set; }

        [CharacterDbfColumn("N_SHET3")]
        public string CounterNumber3 { get; private set; }

        public DbfWaterCustomer()
        {
        }

        public DbfWaterCustomer(int number, string name, string address,
            string counterNumber1, string counterNumber2, string counterNumber3)
        {
            Number = number;
            Name = name ?? string.Empty;
            Address = address ?? string.Empty;
            CounterNumber1 = counterNumber1 ?? string.Empty;
            CounterNumber2 = counterNumber2 ?? string.Empty;
            CounterNumber3 = counterNumber3 ?? string.Empty;
        }
    }
}
