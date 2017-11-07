using CashCenter.DataMigration.Dbf;
using System;

namespace CashCenter.DataMigration.Providers.Dbf.Entities
{
    public class DbfGarbageOrRepairPayment
    {
        [NumericDbfColumn("CODFIN")]
        public int FinancialPeriodCode { get; private set; }

        [DateDbfColumn("DATEPAY", "dd.MM.yyyy")]
        public DateTime CreateDate { get; private set; }

        [DateDbfColumn("TIMEPAY", "HH:mm")]
        public DateTime CreateTime { get; private set; }

        [NumericDbfColumn("NUMCASH")]
        public int FilialCode { get; private set; }

        [NumericDbfColumn("CODORG")]
        public int OrganizationCode { get; private set; }

        [NumericDbfColumn("NUMAB")]
        public int CustomerNumber { get; private set; }

        [MoneyDbfColumn("SUMMA")]
        public decimal Cost { get; private set; }

        public DbfGarbageOrRepairPayment()
        {
        }

        public DbfGarbageOrRepairPayment(int financialPeriodCode, DateTime createDate, DateTime createTime,
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
