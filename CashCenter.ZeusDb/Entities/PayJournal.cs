using System;

namespace CashCenter.ZeusDb.Entities
{
    public class PayJournal
    {
		public int Id { get; private set; }

        public string Name { get; private set; }

        public DateTime CreateDate { get; private set; }

        public int PaymentKindId { get; private set; }

        public PayJournal(int id, string name, DateTime createDate, int paymentKindId)
        {
	        Id = id;
            Name = name;
            CreateDate = createDate;
            PaymentKindId = paymentKindId;
        }

        public PayJournal(string name, DateTime createDate, int paymentKindId)
            : this(-1, name, createDate, paymentKindId)
        {
        }
    }
}
