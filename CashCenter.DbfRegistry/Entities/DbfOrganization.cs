namespace CashCenter.DbfRegistry.Entities
{
    public class DbfOrganization
    {
        public int Id { get; private set; }
        public string DepartamentCode { get; private set; }
        public string ContractNumber { get; private set; }
        public string Name { get; private set; }
        public string Inn { get; private set; }
        public string Kpp { get; private set; }

        public DbfOrganization(int id, string departamentCode,
            string contractNumber, string name, string inn, string kpp)
        {
            Id = id;
            DepartamentCode = departamentCode;
            ContractNumber = contractNumber;
            Name = name;
            Inn = inn;
            Kpp = kpp;
        }
    }
}
