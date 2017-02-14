using CashCenter.Dal;
using System.Collections.Generic;

namespace CashCenter.Articles.DataMigration
{
    public class ImportResult
    {
        public List<Article> AddedArticles { get; private set; }

        public List<Article> UpdatedArticles { get; private set; }

        public List<Article> DeletedArticles { get; private set; }

        public ImportResult()
        {
            AddedArticles = new List<Article>();
            UpdatedArticles = new List<Article>();
            DeletedArticles = new List<Article>();
        }
    }
}