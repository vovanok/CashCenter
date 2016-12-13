using System;

namespace CashCenter.IvEnergySales.DataModel
{
    public class PayJournal
    {
        public string Name { get; private set; }

        public DateTime CreateDate { get; private set; }

        public PaymentKind PaymentKind { get; private set; }

        public PayJournal(string name, DateTime createDate, PaymentKind paymentKind)
        {
            Name = name;
            CreateDate = createDate;
            PaymentKind = paymentKind;
        }
    }
}
