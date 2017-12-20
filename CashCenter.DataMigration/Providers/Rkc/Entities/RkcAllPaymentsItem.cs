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

        internal enum PaymentTarget
        {
            Energy = 1,
            Water = 2,
            Garbage = 3,
            Repair = 4,
            Articles = 5
        }

        public ItemType Type { get; private set; }
        public int OrganizationCode { get; private set; }
        public string Inn { get; private set; }
        public string Kpp { get; private set; }
        public string DepartmentCode { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public decimal PaymentTotal { get; private set; }
        public decimal PaymentCommission { get; private set; }
        public PaymentTarget Target { get; private set; }

        public RkcAllPaymentsItem(ItemType type, int organizationCode, string inn, string kpp,
            string departmentCode, DateTime paymentDate, decimal paymentTotal, decimal paymentCommission,
            PaymentTarget target)
        {
            Type = type;
            OrganizationCode = organizationCode;
            Inn = inn;
            Kpp = kpp;
            DepartmentCode = departmentCode;
            PaymentDate = paymentDate;
            PaymentTotal = paymentTotal;
            PaymentCommission = paymentCommission;
            Target = target;
        }
    }
}
