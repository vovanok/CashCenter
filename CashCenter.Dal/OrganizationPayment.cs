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
    
    public partial class OrganizationPayment
    {
        public int Id { get; set; }
        public int OrganizationId { get; set; }
        public System.DateTime Date { get; set; }
        public int DocumentNumber { get; set; }
        public string Comment { get; set; }
        public decimal Cost { get; set; }
        public int PaymentTypeId { get; set; }
        public string Code1C { get; set; }
        public string IncastCode { get; set; }
        public int PaymentReasonId { get; set; }
    
        public virtual Organization Organization { get; set; }
        public virtual PaymentReason PaymentReason { get; set; }
        public virtual PaymentType PaymentType { get; set; }
    }
}