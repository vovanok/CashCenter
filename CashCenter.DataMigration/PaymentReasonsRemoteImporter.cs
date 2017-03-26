using System;
using System.Linq;
using CashCenter.Common;
using CashCenter.Dal;
using CashCenter.ZeusDb;

namespace CashCenter.DataMigration
{
    public class PaymentReasonsRemoteImporter : BaseRemoteImporter
    {
        public override void Import(Department department)
        {
            if (department == null)
            {
                Log.Error("Для импорта из БД не указано отделение.");
                return;
            }

            try
            {
                DalController.Instance.ClearPaymentReasons();

                var db = new ZeusDbController(department.Code, department.Url, department.Path);
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
