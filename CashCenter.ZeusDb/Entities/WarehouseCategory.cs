namespace CashCenter.ZeusDb.Entities
{
    public class WarehouseCategory
    {
        public int Id { get; private set; }

        public string Code { get; private set; }

        public string Name { get; private set; }

        public bool IsDefault { get; private set; }

        public bool IsWholesale { get; private set; }

        public WarehouseCategory(int id, string code, string name,
            bool isDefault, bool isWholesale)
        {
            Id = id;
            Code = code;
            Name = name;
            IsDefault = isDefault;
            IsWholesale = isWholesale;
        }
    }
}
