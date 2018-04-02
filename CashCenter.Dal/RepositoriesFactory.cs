using System;
using System.Collections.Generic;
using CashCenter.Common;
using CashCenter.Dal.Repositories;

namespace CashCenter.Dal
{
    public static class RepositoriesFactory
    {
        private static CashCenterContext dbContext = new CashCenterContext();

        private static readonly Dictionary<Type, object> repositoriesMap =
            new Dictionary<Type, object>
            {
                { typeof(EnergyCustomerPayment), new EnergyCustomerPaymentRepository(dbContext) },
                { typeof(ArticlePrice), new ArticlePriceRepository(dbContext) },
                { typeof(ArticlePriceType), new ArticlePriceTypeRepository(dbContext) },
                { typeof(Article), new ArticleRepository(dbContext) },
                { typeof(ArticleSale), new ArticleSaleRepository(dbContext) },
                { typeof(Department), new DepartmentRepository(dbContext) },
                { typeof(EnergyCustomer), new EnergyCustomerRepository(dbContext) },
                { typeof(GarbageCollectionPayment), new GarbageCollectionPaymentRepository(dbContext) },
                { typeof(PaymentReason), new PaymentReasonRepository(dbContext) },
                { typeof(Region), new RegionRepository(dbContext) },
                { typeof(RepairPayment), new RepairPaymentRepository(dbContext) },
                { typeof(WaterCustomer), new WaterCustomerRepository(dbContext) },
                { typeof(HotWaterCustomer), new HotWaterCustomerRepository(dbContext) },
                { typeof(HotWaterPayment), new HotWaterPaymentRepository(dbContext) },
            };

        public static IRepository<T> Get<T>() where T: class
        {
            if (!repositoriesMap.ContainsKey(typeof(T)))
                return null;

            return repositoriesMap[typeof(T)] as IRepository<T>;
        }
    }
}
