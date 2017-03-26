namespace CashCenter.ZeusDb.Entities
{
    public class ZeusPaymentKind
    {
        public int Id { get; private set; }

        public string Name { get; private set; }

        public int TypeId { get; private set; }

        public ZeusPaymentKind(int id, string name, int typeId)
        {
            Id = id;
            Name = name;
            TypeId = typeId;
        }
    }
}
