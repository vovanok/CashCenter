using System.Collections.Generic;
using System.Linq;
using CashCenter.DataMigration.Import;
using CashCenter.DataMigration.Providers.Csv;

namespace CashCenter.Objective.HotWater
{
    public class HotWaterCustomerTxtImporter : BaseImporter<TxtHotWaterCustomerModel, HotWaterCustomer>
    {
        public string CsvFilename { get; set; }

        private HotWaterDb db = new HotWaterDb();
        private CsvController csvRegistry;

        public override ImportResult Import()
        {
            csvRegistry = new CsvController(CsvFilename, line => line.StartsWith("#"));
            return base.Import();
        }

        protected override void CreateNewItems(IEnumerable<HotWaterCustomer> newCustomers)
        {
            db.HotWaterCustomers.AddRange(newCustomers);
            db.SaveChanges();
        }

        protected override int DeleteAllTargetItems()
        {
            int count = 0;
            foreach (var hotWaterCustomer in db.HotWaterCustomers)
            {
                hotWaterCustomer.IsActive = false;
                count++;
            }

            db.SaveChanges();

            return count;
        }

        protected override IEnumerable<TxtHotWaterCustomerModel> GetSourceItems()
        {
            var rows = csvRegistry.LoadRows();

            return rows.Select(row =>
            {
                var columns = row.ToArray();

                return new TxtHotWaterCustomerModel(
                    number: GetIntOrDefault(columns, 0),
                    name: GetStringOrDefault(columns, 1),
                    address: GetStringOrDefault(columns, 2),
                    timePeriodCode: GetStringOrDefault(columns, 3),
                    totalForPay: GetDecimalOrDefault(columns, 4),
                    counterName1: GetStringOrDefault(columns, 5),
                    counterValue1: GetDoubleOrDefault(columns, 6),
                    counterName2: GetStringOrDefault(columns, 7),
                    counterValue2: GetDoubleOrDefault(columns, 8),
                    counterName3: GetStringOrDefault(columns, 9),
                    counterValue3: GetDoubleOrDefault(columns, 10),
                    counterName4: GetStringOrDefault(columns, 11),
                    counterValue4: GetDoubleOrDefault(columns, 12),
                    counterName5: GetStringOrDefault(columns, 13),
                    counterValue5: GetDoubleOrDefault(columns, 14));
            });
        }

        protected override HotWaterCustomer GetTargetItemBySource(TxtHotWaterCustomerModel sourceItem)
        {
            return new HotWaterCustomer
            {
                Number = sourceItem.Number,
                Name = sourceItem.Name,
                Address = sourceItem.Address,
                TimePeriodCode = sourceItem.TimePeriodCode,
                TotalForPay = sourceItem.TotalForPay,
                CounterName1 = sourceItem.CounterName1,
                CounterValue1 = sourceItem.CounterValue1,
                CounterName2 = sourceItem.CounterName2,
                CounterValue2 = sourceItem.CounterValue2,
                CounterName3 = sourceItem.CounterName3,
                CounterValue3 = sourceItem.CounterValue3,
                CounterName4 = sourceItem.CounterName4,
                CounterValue4 = sourceItem.CounterValue4,
                CounterName5 = sourceItem.CounterName5,
                CounterValue5 = sourceItem.CounterValue5,
                Email = string.Empty,
                IsActive = true
            };
        }

        protected override bool TryUpdateExistingItem(TxtHotWaterCustomerModel txtHotWaterCustomer)
        {
            if (txtHotWaterCustomer == null)
                return false;

            var existingHotWaterCustomer = db.HotWaterCustomers.FirstOrDefault(customer => customer.Number == txtHotWaterCustomer.Number);
            if (existingHotWaterCustomer == null)
                return false;

            existingHotWaterCustomer.Name = txtHotWaterCustomer.Name;
            existingHotWaterCustomer.Address = txtHotWaterCustomer.Address;
            existingHotWaterCustomer.TimePeriodCode = txtHotWaterCustomer.TimePeriodCode;
            existingHotWaterCustomer.TotalForPay = txtHotWaterCustomer.TotalForPay;
            existingHotWaterCustomer.CounterName1 = txtHotWaterCustomer.CounterName1;
            existingHotWaterCustomer.CounterValue1 = txtHotWaterCustomer.CounterValue1;
            existingHotWaterCustomer.CounterName2 = txtHotWaterCustomer.CounterName2;
            existingHotWaterCustomer.CounterValue2 = txtHotWaterCustomer.CounterValue2;
            existingHotWaterCustomer.CounterName3 = txtHotWaterCustomer.CounterName3;
            existingHotWaterCustomer.CounterValue3 = txtHotWaterCustomer.CounterValue3;
            existingHotWaterCustomer.CounterName4 = txtHotWaterCustomer.CounterName4;
            existingHotWaterCustomer.CounterValue4 = txtHotWaterCustomer.CounterValue4;
            existingHotWaterCustomer.CounterName5 = txtHotWaterCustomer.CounterName5;
            existingHotWaterCustomer.CounterValue5 = txtHotWaterCustomer.CounterValue5;
            existingHotWaterCustomer.IsActive = true;

            db.SaveChanges();
            return true;
        }

        private int GetIntOrDefault(string[] array, int index)
        {
            if (index >= array.Length)
                return 0;

            if (!int.TryParse(array[index], out int intValue))
                return 0;

            return intValue;
        }

        private decimal GetDecimalOrDefault(string[] array, int index)
        {
            if (index >= array.Length)
                return 0;

            if (!decimal.TryParse(array[index], out decimal decimalValue))
                return 0;

            return decimalValue;
        }

        private double GetDoubleOrDefault(string[] array, int index)
        {
            if (index >= array.Length)
                return 0;

            if (!double.TryParse(array[index], out double doubleValue))
                return 0;

            return doubleValue;
        }

        private string GetStringOrDefault(string[] array, int index)
        {
            if (index >= array.Length)
                return string.Empty;

            return array[index];
        }
    }
}
