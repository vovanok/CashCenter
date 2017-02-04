using System;

namespace CashCenter.Common.DataEntities
{
    public class OrganizationPayment
    {
        public Organization Organization { get; private set; }
        public string DocumentNumber { get; private set; }
        public string DepartmentCode { get; private set; }
        public DateTime CreateDate { get; private set; }
        public string Comment { get; private set; }
        public decimal Cost { get; private set; }
        public int PaymentTypeId { get; private set; }
        public string PaymentTypeName { get; private set; }
        public string Code1C { get; private set; }
        public string IncastCodePayment { get; private set; }
        public int ReasonId { get; private set; }

        public OrganizationPayment(Organization organization, string documentNumber, string departmentCode,
            DateTime createDate, string comment, decimal cost, int paymentTypeId, string paymentTypeName,
            string code1C, string incastCodePayment, int reasonId)
        {
            Organization = organization;
            DocumentNumber = documentNumber;
            DepartmentCode = departmentCode;
            CreateDate = createDate;
            Comment = comment;
            Cost = cost;
            PaymentTypeId = paymentTypeId;
            PaymentTypeName = paymentTypeName;
            Code1C = code1C;
            IncastCodePayment = incastCodePayment;
            ReasonId = reasonId;
        }
    }
}
