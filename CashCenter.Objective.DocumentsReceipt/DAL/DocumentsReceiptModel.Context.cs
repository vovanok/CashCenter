﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CashCenter.Objective.DocumentsReceipt.Dal
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CashCenterDocumentsReceiptEntities : DbContext
    {
        public CashCenterDocumentsReceiptEntities()
            : base("name=CashCenterDocumentsReceiptEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<ReceiptDocument> ReceiptDocuments { get; set; }
        public virtual DbSet<SettlementCenter> SettlementCenters { get; set; }
    }
}
