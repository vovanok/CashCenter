using System;

namespace CashCenter.DataMigration.Providers.Rkc.Entities
{
    internal class RkcAllPaymentsItem
    {
        internal enum ItemType
        {
            Payment = 1,
            Encashment = 2
        }

        public ItemType Type { get; private set; }
        public int OrganizationCode { get; private set; }
        public string Inn { get; private set; }
        public string Kpp { get; private set; }
        public string DepartmentCode { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public decimal PaymentTotal { get; private set; }
        public decimal PaymentCommission { get; private set; }

        public RkcAllPaymentsItem(ItemType type, int organizationCode, string inn, string kpp,
            string departmentCode, DateTime paymentDate, decimal paymentTotal, decimal paymentCommission)
        {
            Type = type;
            OrganizationCode = organizationCode;
            Inn = inn;
            Kpp = kpp;
            DepartmentCode = departmentCode;
            PaymentDate = paymentDate;
            PaymentTotal = paymentTotal;
            PaymentCommission = paymentCommission;
        }
    }
}
