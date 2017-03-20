using System;
using System.Linq;
using CashCenter.Common.DbQualification;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.ZeusDb;

namespace CashCenter.DataMigration
{
    public class PaymentKindsRemoteImporter : BaseRemoteImporter
    {
        public override void Import(DepartmentDef departmentDef)
        {
            if (departmentDef == null)
            {
                Log.Error("Для импорта из БД не указано отделение.");
                return;
            }

            try
            {
                DalController.Instance.ClearPaymentKinds();

                var db = new ZeusDbController(departmentDef);
                var importedPaymentKinds = db.GetPaymentKinds();
                var paymentKindsForAdd = importedPaymentKinds.Select(paymentKind => new PaymentKind
                    {
                        Id = paymentKind.Id,
                        Name = paymentKind.Name
                    });

                DalController.Instance.AddPaymentKindsRange(paymentKindsForAdd);
            }
            catch (Exception ex)
            {
                Log.ErrorWithException("Ошибка импортирования данных из удаленной БД", ex);
            }
        }
    }
}
