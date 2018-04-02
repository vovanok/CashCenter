using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using CashCenter.Common;

namespace CashCenter.Dal.Repositories
{
    public abstract class BaseEfRepository<T, TContext>
        : IRepository<T> where T: class where TContext : DbContext
    {
        private TContext context;

        protected abstract DbSet<T> GetDdSet(TContext context);

        public BaseEfRepository(TContext context)
        {
            this.context = context;
        }

        public void Add(T item)
        {
            if (item == null)
                return;

            try
            {
                GetDdSet(context).Add(item);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError($"Add {typeof(T).Name}", ex);
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            if (items == null)
                return;

            try
            {
                GetDdSet(context).AddRange(items);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError($"Add range {typeof(T).Name}", ex);
            }
        }

        public void Delete(T item)
        {
            if (item == null)
                return;

            try
            {
                GetDdSet(context).Remove(item);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError($"Delete {typeof(T).Name}", ex);
            }
        }

        public void DeleteRange(IEnumerable<T> items)
        {
            if (items == null)
                return;

            try
            {
                GetDdSet(context).RemoveRange(items);
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError($"Delete range {typeof(T).Name}", ex);
            }
        }

        public T Get(Func<T, bool> filter)
        {
            if (filter == null)
                return null;

            try
            {
                return GetDdSet(context).FirstOrDefault(filter);
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError($"Get {typeof(T).Name}", ex);
                return null;
            }
        }

        public IEnumerable<T> GetAll()
        {
            try
            {
                return GetDdSet(context);
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError($"Get all {typeof(T).Name}", ex);
                return null;
            }
        }

        public IEnumerable<T> Filter(Func<T, bool> filter)
        {
            if (filter == null)
                return null;

            try
            {
                return GetDdSet(context).Where(filter);
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError($"Get by filter {typeof(T).Name}", ex);
                return null;
            }
        }

        public void Update(T item)
        {
            if (item == null)
                return;

            try
            {
                if (GetDdSet(context).Contains(item))
                    context.SaveChanges();
            }
            catch (Exception ex)
            {
                HandleEntityFrameworkError($"Update {typeof(T).Name}", ex);
            }
        }

        public void RollbackItemsChanges()
        {
            foreach (var entry in context.ChangeTracker.Entries<T>().Where(entry => entry != null))
            {
                entry.State = EntityState.Detached;
            }
        }

        private void HandleEntityFrameworkError(string placeName, Exception exception)
        {
            var entityValidationException = exception as DbEntityValidationException;
            if (entityValidationException == null)
            {
                if (exception.InnerException != null)
                {
                    HandleEntityFrameworkError(placeName, exception.InnerException);
                    return;
                }

                Log.Error(placeName, exception);
                return;
            }

            var sbErrorContent = new StringBuilder();
            foreach (var entityValidationError in entityValidationException.EntityValidationErrors)
            {
                sbErrorContent.AppendLine($"Таблица {entityValidationError.Entry.Entity.GetType().Name} в состоянии {entityValidationError.Entry.State} ошибки:");
                foreach (var validationError in entityValidationError.ValidationErrors)
                {
                    sbErrorContent.AppendLine($"\tСвойство: {validationError.PropertyName}, ошибка: {validationError.ErrorMessage}");
                }
            }

            Log.Error($"Error in '{placeName}':\n{sbErrorContent}", exception);
        }
    }
}