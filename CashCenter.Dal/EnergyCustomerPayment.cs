//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CashCenter.Dal
{
    using System;
    using System.Collections.Generic;
    
    public partial class EnergyCustomerPayment
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int NewDayValue { get; set; }
        public int NewNightValue { get; set; }
        public decimal Cost { get; set; }
        public int ReasonId { get; set; }
        public System.DateTime CreateDate { get; set; }
        public string Description { get; set; }
        public int FiscalNumber { get; set; }
    
        public virtual PaymentReason PaymentReason { get; set; }
        public virtual EnergyCustomer EnergyCustomer { get; set; }
    }
}
