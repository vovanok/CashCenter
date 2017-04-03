using System.Collections.Generic;
using System.Linq;
using CashCenter.Dal;
using CashCenter.ZeusDb.Entities;

namespace CashCenter.DataMigration
{
    public class PaymentKindsRemoteImporter : BaseRemoteImporter<ZeusPaymentKind, PaymentKind>
    {
        protected override int DeleteAllTargetItems()
        {
            return 0;
        }

        protected override IEnumerable<ZeusPaymentKind> GetSourceItems()
        {
            if (db == null)
                return new List<ZeusPaymentKind>();

            return db.GetPaymentKinds();
        }

        protected override bool TryUpdateExistingItem(ZeusPaymentKind zeusPaymentKind)
        {
            var existingPaymentKind = DalController.Instance.PaymentKinds
                .FirstOrDefault(paymentKind => paymentKind.Id == zeusPaymentKind.Id);

            if (existingPaymentKind == null)
                return false;

            existingPaymentKind.Name = zeusPaymentKind.Name;
            existingPaymentKind.TypeZeusId = zeusPaymentKind.TypeId;

            return true;
        }

        protected override void CreateNewItems(IEnumerable<PaymentKind> paymentKinds)
        {
            DalController.Instance.AddPaymentKindsRange(paymentKinds);
        }

        protected override PaymentKind GetTargetItemBySource(ZeusPaymentKind zeusPaymentKind)
        {
            return new PaymentKind
            {
                Id = zeusPaymentKind.Id,
                Name = zeusPaymentKind.Name,
                TypeZeusId = zeusPaymentKind.Id
            };
        }
    }
}
