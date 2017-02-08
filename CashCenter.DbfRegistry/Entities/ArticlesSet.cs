using System;
using System.Collections.Generic;

namespace CashCenter.DbfRegistry.Entities
{
    public class ArticlesSet
    {
        public DateTime EntryDate { get; private set; }

        public List<Article> Articles { get; private set; }

        public ArticlesSet()
        {
            EntryDate = DateTime.Now;
            Articles = new List<Article>();
        }

        public ArticlesSet(DateTime entryDate, List<Article> articles)
        {
            EntryDate = entryDate;
            Articles = articles ?? new List<Article>();
        }
    }
}
