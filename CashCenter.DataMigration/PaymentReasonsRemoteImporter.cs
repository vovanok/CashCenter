using System;
using System.Linq;
using CashCenter.Common.DbQualification;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.ZeusDb;

namespace CashCenter.DataMigration
{
    public class PaymentReasonsRemoteImporter : BaseRemoteImporter
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
                DalController.Instance.ClearPaymentReasons();

                var db = new ZeusDbController(departmentDef);
                var importedPaymentReasons = db.GetPaymentReasons();
                var paymentReasonsForAdd = importedPaymentReasons.Select(paymentReason => new PaymentReason
                {
                    Id = paymentReason.Id,
                    Name = paymentReason.Name
                });

                DalController.Instance.AddPaymentReasonsRange(paymentReasonsForAdd);
            }
            catch (Exception ex)
            {
                Log.ErrorWithException("Ошибка импортирования данных из удаленной БД", ex);
            }
        }
    }
}
