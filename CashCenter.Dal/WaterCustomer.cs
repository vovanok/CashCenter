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
    
    public partial class WaterCustomer
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public WaterCustomer()
        {
            this.WaterCustomerPayments = new HashSet<WaterCustomerPayment>();
        }
    
        public int Id { get; set; }
        public int Number { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string CounterNumber1 { get; set; }
        public string CounterNumber2 { get; set; }
        public string CounterNumber3 { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WaterCustomerPayment> WaterCustomerPayments { get; set; }
    }
}
