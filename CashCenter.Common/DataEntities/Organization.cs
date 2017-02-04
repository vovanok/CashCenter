namespace CashCenter.Common.DataEntities
{
    public class Organization
    {
        public int Id { get; private set; }
        public string DepartamentCode { get; private set; }
        public string ContractNumber { get; private set; }
        public string Name { get; private set; }
        public string Inn { get; private set; }
        public string Kpp { get; private set; }

        public Organization(int id, string departamentCode,
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
