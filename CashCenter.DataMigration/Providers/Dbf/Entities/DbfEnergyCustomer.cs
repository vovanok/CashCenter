using CashCenter.DataMigration.Dbf;

namespace CashCenter.DataMigration.Providers.Dbf.Entities
{
    public class DbfEnergyCustomer
    {
        [NumericDbfColumn("LS")]
        public int Number { get; private set; }

        [CharacterDbfColumn("NAME")]
        public string Name { get; private set; }

        [CharacterDbfColumn("ADDRESS")]
        public string Address { get; private set; }

        [CharacterDbfColumn("KO")]
        public string DepartmentCode { get; private set; }

        [NumericDbfColumn("CDAY")]
        public int DayValue { get; private set; }

        [NumericDbfColumn("CNIGHT")]
        public int NightValue { get; private set; }
        
        [MoneyDbfColumn("SUMMA")]
        public decimal Balance { get; private set; }

        [BooleanDbfColumn("ISCLOSED")]
        public bool IsClosed { get; private set; }

        [CharacterDbfColumn("DOCID")]
        public string PaymentDocumentIdentifier { get; private set; }

        [CharacterDbfColumn("HUSID")]
        public string HusIdentifier { get; private set; }

        public DbfEnergyCustomer()
        {
        }

        public DbfEnergyCustomer(int number, string name, string address, string departmentCode, int dayValue,
            int nightValue, decimal balance, bool isClosed, string paymentDocumentIdentifier, string husIdentifier)
        {
            Number = number;
            Name = name;
            Address = address;
            DepartmentCode = departmentCode;
            DayValue = dayValue;
            NightValue = nightValue;
            Balance = balance;
            IsClosed = isClosed;
            PaymentDocumentIdentifier = paymentDocumentIdentifier;
            HusIdentifier = husIdentifier;
        }
    }
}
