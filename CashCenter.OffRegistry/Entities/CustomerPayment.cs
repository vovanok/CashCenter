using System;

namespace CashCenter.OffRegistry.Entities
{
    public class CustomerPayment
    {
        public string Id { get; private set; }
        public int CustomerId { get; private set; }
        public int DayValue { get; private set; }
        public int NightValue { get; private set; }
        public int ReasonId { get; private set; }
        public decimal TotalCost { get; private set; }
        public DateTime CreateDate { get; private set; }
        public string DepartmentCode { get; private set; }

        public CustomerPayment(string id, int customerId, int dayValue, int nightValue,
            int reasonId, decimal totalCost, DateTime createDate, string departmentCode)
        {
            Id = id;
            CustomerId = customerId;
            DayValue = dayValue;
            NightValue = nightValue;
            ReasonId = reasonId;
            TotalCost = totalCost;
            CreateDate = createDate;
            DepartmentCode = departmentCode;
        }
    }
}
