using System;

namespace CashCenter.DataMigration.Providers.Firebird.Entities
{
    public class ZeusPayJournal
    {
		public int Id { get; private set; }
        public string Name { get; private set; }
        public DateTime CreateDate { get; private set; }
        public int PaymentKindId { get; private set; }

        public ZeusPayJournal(int id, string name, DateTime createDate, int paymentKindId)
        {
	        Id = id;
            Name = name;
            CreateDate = createDate;
            PaymentKindId = paymentKindId;
        }

        public ZeusPayJournal(string name, DateTime createDate, int paymentKindId)
            : this(-1, name, createDate, paymentKindId)
        {
        }
    }
}
