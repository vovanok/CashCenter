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
    
    public partial class ArticleSale
    {
        public int Id { get; set; }
        public int ArticlePriceId { get; set; }
        public double Quantity { get; set; }
        public System.DateTime Date { get; set; }
    
        public virtual ArticlePrice ArticlePrice { get; set; }
    }
}