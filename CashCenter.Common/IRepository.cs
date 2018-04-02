using System;
using System.Collections.Generic;

namespace CashCenter.Common
{
    public interface IRepository<T> where T: class
    {
        T Get(Func<T, bool> condition);
        IEnumerable<T> GetAll();
        IEnumerable<T> Filter(Func<T, bool> condition);
        void Add(T item);
        void AddRange(IEnumerable<T> items);
        void Delete(T item);
        void DeleteRange(IEnumerable<T> items);
        void Update(T item);
        void RollbackItemsChanges();
    }
}
