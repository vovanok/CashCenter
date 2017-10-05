using System;

namespace CashCenter.DataMigration.Providers.Dbf.Entities
{
    public class DbfGarbageCollectionPayment
    {
        public int FinancialPeriodCode { get; private set; }
        public DateTime CreateDate { get; private set; }
        public int FilialCode { get; private set; }
        public int OrganizationCode { get; private set; }
        public int CustomerNumber { get; private set; }
        public decimal Cost { get; private set; }

        public DbfGarbageCollectionPayment(int financialPeriodCode, DateTime createDate,
            int filialCode, int organizationCode, int customerNumber, decimal cost)
        {
            FinancialPeriodCode = financialPeriodCode;
            CreateDate = createDate;
            FilialCode = filialCode;
            OrganizationCode = organizationCode;
            CustomerNumber = customerNumber;
            Cost = cost;
        }
    }
}
