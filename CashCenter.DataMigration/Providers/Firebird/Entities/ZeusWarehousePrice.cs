using System;

namespace CashCenter.DataMigration.Providers.Firebird.Entities
{
    public class ZeusWarehousePrice
    {
        public int Id { get; private set; }

        public int WarehouseId { get; private set; }

        public int WarehouseCategoryId { get; private set; }

        public DateTime EntryDate { get; private set; }

        public decimal PriceValue { get; private set; }

        public ZeusWarehousePrice(int id, int warehouseId, int warehouseCategoryId,
            DateTime entryDate, decimal priceValue)
        {
            Id = id;
            WarehouseId = warehouseId;
            WarehouseCategoryId = warehouseCategoryId;
            EntryDate = entryDate;
            PriceValue = priceValue;
        }
    }
}
