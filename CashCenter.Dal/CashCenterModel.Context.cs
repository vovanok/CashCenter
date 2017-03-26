﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class CashCenterContext : DbContext
    {
        public CashCenterContext()
            : base("name=CashCenterContext")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ArticlePrice> ArticlePrices { get; set; }
        public virtual DbSet<ArticleSale> ArticleSales { get; set; }
        public virtual DbSet<Organization> Organizations { get; set; }
        public virtual DbSet<OrganizationPayment> OrganizationPayments { get; set; }
        public virtual DbSet<CustomerPayment> CustomerPayments { get; set; }
        public virtual DbSet<PaymentReason> PaymentReasons { get; set; }
        public virtual DbSet<PaymentKind> PaymentKinds { get; set; }
        public virtual DbSet<Article> Articles { get; set; }
        public virtual DbSet<ArticlePriceType> ArticlePriceTypes { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Region> Regions { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
    }
}
