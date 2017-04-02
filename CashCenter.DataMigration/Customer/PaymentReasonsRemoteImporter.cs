using System.Collections.Generic;
using System.Linq;
using CashCenter.Dal;
using CashCenter.ZeusDb.Entities;

namespace CashCenter.DataMigration
{
    public class PaymentReasonsRemoteImporter : BaseRemoteImporter<ZeusPaymentReason, PaymentReason>
    {
        protected override void CreateNewItems(IEnumerable<PaymentReason> paymentReasons)
        {
            DalController.Instance.AddPaymentReasonsRange(paymentReasons);
        }

        protected override int DeleteAllTargetItems()
        {
            return 0;
        }

        protected override IEnumerable<ZeusPaymentReason> GetSourceItems()
        {
            if (db == null)
                return new List<ZeusPaymentReason>();

            return db.GetPaymentReasons();
        }

        protected override PaymentReason GetTargetItemBySource(ZeusPaymentReason zeusPaymentReason)
        {
            return new PaymentReason
                {
                    Id = zeusPaymentReason.Id,
                    Name = zeusPaymentReason.Name
                };
        }

        protected override bool TryUpdateExistingItem(ZeusPaymentReason zeusPaymentReason)
        {
            var existingPaymentReason = DalController.Instance.PaymentReasons
                .FirstOrDefault(paymentReason => paymentReason.Id == zeusPaymentReason.Id);

            if (existingPaymentReason == null)
                return false;

            existingPaymentReason.Name = zeusPaymentReason.Name;

            return true;
        }
    }
}
