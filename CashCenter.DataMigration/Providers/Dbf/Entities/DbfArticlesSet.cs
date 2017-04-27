using System;
using System.Collections.Generic;

namespace CashCenter.DataMigration.Providers.Dbf.Entities
{
    public class DbfArticlesSet
    {
        public DateTime EntryDate { get; private set; }

        public List<DbfArticle> Articles { get; private set; }

        public DbfArticlesSet()
        {
            EntryDate = DateTime.Now;
            Articles = new List<DbfArticle>();
        }

        public DbfArticlesSet(DateTime entryDate, List<DbfArticle> articles)
        {
            EntryDate = entryDate;
            Articles = articles ?? new List<DbfArticle>();
        }
    }
}
