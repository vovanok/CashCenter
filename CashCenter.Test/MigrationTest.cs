using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CashCenter.DataMigration.Providers.Dbf.Entities;
using CashCenter.DataMigration.Dbf;
using System.Diagnostics;

namespace CashCenter.Test
{
    [TestClass]
    public class MigrationTest
    {
        [TestMethod]
        public void DbfReadWriteTest()
        {
            var savedArticles = new List<DbfArticle>
            {
                new DbfArticle("code1", "name1", "barcode1", 100, DateTime.Now),
                new DbfArticle(string.Empty, null, "barcode2", -34, DateTime.MinValue)
            };

            var savedArtileQuntities = new List<DbfArticleQuantity>
            {
                new DbfArticleQuantity("code1", 123, "measure1"),
                new DbfArticleQuantity(null, -665, string.Empty)
            };

            var savedArticleSales = new List<DbfArticleSale>
            {
                new DbfArticleSale(DateTime.Now, "docnum1", "whcode1", "whname1", "artcode", "artname", 9, 543, 34, 222, "sernum1", "comment1"),
                new DbfArticleSale(DateTime.MinValue, null, null, null, null, null, -30, 434, -45, 5, null, null)
            };

            var savedEnergyCustomers = new List<DbfEnergyCustomer>
            {
                new DbfEnergyCustomer(123, "name1", "address1", "depcode1", 345, 555, (decimal)56.56, false),
                new DbfEnergyCustomer(-40, null, string.Empty, null, -665, 55, 0, true)
            };

            var savedGarbageAndRepairsPayments = new List<DbfGarbageOrRepairPayment>
            {
                new DbfGarbageOrRepairPayment(54, DateTime.Now, DateTime.Now, 3434, 5555, 3434, (decimal)2343.34),
                new DbfGarbageOrRepairPayment(-34, DateTime.MinValue, DateTime.MaxValue, 3434, -555, 343, 0)
            };

            var savedWaterCustomers = new List<DbfWaterCustomer>
            {
                new DbfWaterCustomer(23, "name1", "address1", "countnum11", "countnum21", "countnum31"),
                new DbfWaterCustomer(-444, null, null, null, string.Empty, null)
            };

            var savedWaterCustomerPayments = new List<DbfWaterCustomerPayment>
            {
                new DbfWaterCustomerPayment(DateTime.Now, 35, (decimal)(-455.66), "percode1", (decimal)45333.3,
                    "countnum11", 454, 55,
                    "countnum21", 44, 23222,
                    "countnum31", 48754, 3434,
                    "countnum41", -55, 77654),
                new DbfWaterCustomerPayment(DateTime.Now, 35, (decimal)(-455.66), null, (decimal)45333.3,
                    null, 454, 55,
                    "countnum22", 4, 23222,
                    null, 48754, 3434,
                    "countnum42", -55, 77654)
            };

            Debug.WriteLine(">>> DbfArticle to DBF");
            Dbf.SaveToFile(savedArticles, "arts.dbf");

            Debug.WriteLine(">>> DbfArticleQuantity to DBF");
            Dbf.SaveToFile(savedArtileQuntities, "arqu.dbf");

            Debug.WriteLine(">>> DbfArticleSale to DBF");
            Dbf.SaveToFile(savedArticleSales, "arsal.dbf");

            Debug.WriteLine(">>> DbfEnergyCustomer to DBF");
            Dbf.SaveToFile(savedEnergyCustomers, "encu.dbf");

            Debug.WriteLine(">>> DbfGarbageOrRepairPayment to DBF");
            Dbf.SaveToFile(savedGarbageAndRepairsPayments, "garp.dbf");

            Debug.WriteLine(">>> DbfWaterCustomer to DBF");
            Dbf.SaveToFile(savedWaterCustomers, "wacu.dbf");

            Debug.WriteLine(">>> DbfWaterCustomerPayment to DBF");
            Dbf.SaveToFile(savedWaterCustomerPayments, "wacup.dbf");


            var loadedArticles = Dbf.LoadFromFile<DbfArticle>("arts.dbf");
            for (int i = 0; i < loadedArticles.Count; i++)
            {
                var loaded = loadedArticles[i];
                var saved = savedArticles[i];

                Assert.IsTrue(loaded.Code == saved.Code, "Articles code");
                Assert.IsTrue(loaded.Name == saved.Name, "Articles name");
                Assert.IsTrue(loaded.Barcode == saved.Barcode, "Articles barcode");
                Assert.IsTrue(loaded.Price == saved.Price, "Articles price");
                Assert.IsTrue(loaded.EntryDate.Date == saved.EntryDate.Date, "Articles EntryDate");
            }

            var loadedArtileQuntities = Dbf.LoadFromFile<DbfArticleQuantity>("arqu.dbf");
            for (int i = 0; i < loadedArtileQuntities.Count; i++)
            {
                var loaded = loadedArtileQuntities[i];
                var saved = savedArtileQuntities[i];

                Assert.IsTrue(loaded.ArticleCode == saved.ArticleCode, "ArticleQuantity ArticleCode");
                Assert.IsTrue(loaded.Quantity == saved.Quantity, "ArticleQuantity Quantity");
                Assert.IsTrue(loaded.Measure == saved.Measure, "ArticleQuantity Measure");
            }

            var loadedArticleSales = Dbf.LoadFromFile<DbfArticleSale>("arsal.dbf");
            for (int i = 0; i < loadedArticleSales.Count; i++)
            {
                var loaded = loadedArticleSales[i];
                var saved = savedArticleSales[i];

                Assert.IsTrue(loaded.DocumentDateTime == saved.DocumentDateTime, "ArticleSale DocumentDateTime");
                Assert.IsTrue(loaded.DocumentNumber == saved.DocumentNumber, "ArticleSale DocumentNumber");
                Assert.IsTrue(loaded.WarehouseCode == saved.WarehouseCode, "ArticleSale WarehouseCode");
                Assert.IsTrue(loaded.WarehouseName == saved.WarehouseName, "ArticleSale WarehouseName");
                Assert.IsTrue(loaded.ArticleCode == saved.ArticleCode, "ArticleSale ArticleCode");
                Assert.IsTrue(loaded.ArticleName == saved.ArticleName, "ArticleSale ArticleName");
                Assert.IsTrue(loaded.ArticleQuantity == saved.ArticleQuantity, "ArticleSale ArticleQuantity");
                Assert.IsTrue(loaded.ArticlePrice == saved.ArticlePrice, "ArticleSale ArticlePrice");
                Assert.IsTrue(loaded.ArticleTotalPrice == saved.ArticleTotalPrice, "ArticleSale ArticleTotalPrice");
                Assert.IsTrue(loaded.CheckNumber == saved.CheckNumber, "ArticleSale CheckNumber");
                Assert.IsTrue(loaded.SerialNumber == saved.SerialNumber, "ArticleSale SerialNumber");
                Assert.IsTrue(loaded.Comment == saved.Comment, "ArticleSale Comment");
            }

            var loadedEnergyCustomers = Dbf.LoadFromFile<DbfEnergyCustomer>("encu.dbf");
            for (int i = 0; i < loadedEnergyCustomers.Count; i++)
            {
                var loaded = loadedEnergyCustomers[i];
                var saved = savedEnergyCustomers[i];

                Assert.IsTrue(loaded.Number == saved.Number, "EnergyCustomer Number");
                Assert.IsTrue(loaded.Name == saved.Name, "EnergyCustomer Name");
                Assert.IsTrue(loaded.Address == saved.Address, "EnergyCustomer Address");
                Assert.IsTrue(loaded.DepartmentCode == saved.DepartmentCode, "EnergyCustomer DepartmentCode");
                Assert.IsTrue(loaded.DayValue == saved.DayValue, "EnergyCustomer DayValue");
                Assert.IsTrue(loaded.NightValue == saved.NightValue, "EnergyCustomer NightValue");
                Assert.IsTrue(loaded.Balance == saved.Balance, "EnergyCustomer Balance");
                Assert.IsTrue(loaded.IsClosed == saved.IsClosed, "EnergyCustomer IsClosed");
            }

            var loadedGarbageAndRepairsPayments = Dbf.LoadFromFile<DbfGarbageOrRepairPayment>("garp.dbf");
            for (int i = 0; i < loadedGarbageAndRepairsPayments.Count; i++)
            {
                var loaded = loadedGarbageAndRepairsPayments[i];
                var saved = savedGarbageAndRepairsPayments[i];

                Assert.IsTrue(loaded.FinancialPeriodCode == saved.FinancialPeriodCode, "GarbageOrRepairPayment FinancialPeriodCode");
                Assert.IsTrue(loaded.CreateDate == saved.CreateDate, "GarbageOrRepairPayment CreateDate");
                Assert.IsTrue(loaded.CreateTime == saved.CreateTime, "GarbageOrRepairPayment CreateTime");
                Assert.IsTrue(loaded.FilialCode == saved.FilialCode, "GarbageOrRepairPayment FilialCode");
                Assert.IsTrue(loaded.OrganizationCode == saved.OrganizationCode, "GarbageOrRepairPayment OrganizationCode");
                Assert.IsTrue(loaded.CustomerNumber == saved.CustomerNumber, "GarbageOrRepairPayment CustomerNumber");
                Assert.IsTrue(loaded.Cost == saved.Cost, "GarbageOrRepairPayment Cost");
            }

            var loadedWaterCustomers = Dbf.LoadFromFile<DbfWaterCustomer>("wacu.dbf");
            for (int i = 0; i < loadedWaterCustomers.Count; i++)
            {
                var loaded = loadedWaterCustomers[i];
                var saved = savedWaterCustomers[i];

                Assert.IsTrue(loaded.Number == saved.Number, "WaterCustomer Number");
                Assert.IsTrue(loaded.Name == saved.Name, "WaterCustomer Name");
                Assert.IsTrue(loaded.Address == saved.Address, "WaterCustomer Address");
                Assert.IsTrue(loaded.CounterNumber1 == saved.CounterNumber1, "WaterCustomer CounterNumber1");
                Assert.IsTrue(loaded.CounterNumber2 == saved.CounterNumber2, "WaterCustomer CounterNumber2");
                Assert.IsTrue(loaded.CounterNumber3 == saved.CounterNumber3, "WaterCustomer CounterNumber3");
            }

            var loadedWaterCustomerPayments = Dbf.LoadFromFile<DbfWaterCustomerPayment>("wacup.dbf");
            for (int i = 0; i < loadedWaterCustomerPayments.Count; i++)
            {
                var loaded = loadedWaterCustomerPayments[i];
                var saved = savedWaterCustomerPayments[i];

                Assert.IsTrue(loaded.CreationDate == saved.CreationDate, "WaterCustomerPayment CreationDate");
                Assert.IsTrue(loaded.CustomerNumber == saved.CustomerNumber, "WaterCustomerPayment CustomerNumber");
                Assert.IsTrue(loaded.Cost == saved.Cost, "WaterCustomerPayment Cost");
                Assert.IsTrue(loaded.PeriodCode == saved.PeriodCode, "WaterCustomerPayment PeriodCode");
                Assert.IsTrue(loaded.Penalty == saved.Penalty, "WaterCustomerPayment Penalty");
                Assert.IsTrue(loaded.CounterNumber1 == saved.CounterNumber1, "WaterCustomerPayment CounterNumber1");
                Assert.IsTrue(loaded.CounterCost1 == saved.CounterCost1, "WaterCustomerPayment CounterCost1");
                Assert.IsTrue(loaded.CounterValue1 == saved.CounterValue1, "WaterCustomerPayment CounterValue1");
                Assert.IsTrue(loaded.CounterNumber2 == saved.CounterNumber2, "WaterCustomerPayment CounterNumber2");
                Assert.IsTrue(loaded.CounterCost2 == saved.CounterCost2, "WaterCustomerPayment CounterCost2");
                Assert.IsTrue(loaded.CounterValue2 == saved.CounterValue2, "WaterCustomerPayment CounterValue2");
                Assert.IsTrue(loaded.CounterNumber3 == saved.CounterNumber3, "WaterCustomerPayment CounterNumber3");
                Assert.IsTrue(loaded.CounterCost3 == saved.CounterCost3, "WaterCustomerPayment CounterCost3");
                Assert.IsTrue(loaded.CounterValue3 == saved.CounterValue3, "WaterCustomerPayment CounterValue3");
                Assert.IsTrue(loaded.CounterNumber4 == saved.CounterNumber4, "WaterCustomerPayment CounterNumber4");
                Assert.IsTrue(loaded.CounterCost4 == saved.CounterCost4, "WaterCustomerPayment CounterCost4");
                Assert.IsTrue(loaded.CounterValue4 == saved.CounterValue4, "WaterCustomerPayment CounterValue4");
            }
        }
    }
}
