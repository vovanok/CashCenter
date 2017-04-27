namespace CashCenter.DataMigration.Providers.Firebird.Entities
{
    public class ZeusWarehouse
    {
        public int Id { get; private set; }

        public string Code { get; private set; }

        public string Name { get; private set; }

        public float Quantity { get; private set; }

        public string UnitName { get; private set; }

        public string Barcode { get; private set; }

        public ZeusWarehouse(int id, string code, string name,
            float quantity, string unitName, string barcode)
        {
            Id = id;
            Code = code;
            Name = name;
            Quantity = quantity;
            UnitName = unitName;
            Barcode = barcode;
        }
    }
}
