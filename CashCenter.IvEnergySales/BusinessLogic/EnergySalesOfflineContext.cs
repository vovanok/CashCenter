using System;
using CashCenter.IvEnergySales.DbQualification;
using System.IO;
using CashCenter.IvEnergySales.Logging;
using System.Linq;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class EnergySalesOfflineContext : BaseEnergySalesContext
    {
        public EnergySalesOfflineContext(int customerId, RegionDef regionDef, string dbCode)
            : base(customerId, regionDef, dbCode)
        {
        }

        public override bool Pay(int reasonId, int paymentKindId, int value1, int value2, decimal cost, string description, DateTime createDate)
        {
            try
            {
                if (!IsCustomerFinded)
                {
                    Log.Error($"Ошибка совершения платежа. Отсутствует плательщик.");
                    return false;
                }

                if (!ValidateReasonId(reasonId))
                    return false;

                if (!ValidateDayCounterValue(value1))
                    return false;

                if (!ValidateNightCounterValue(value2))
                    return false;

                if (!ValidateCost(cost))
                    return false;

                var lineForWrite = $"CustID = {Customer.Id}; Day = {value1}; Night = {value2};; ReasonID = {reasonId}; Total = {cost}; id = {Guid.NewGuid()}; date = {createDate.ToString("dd.MM.yyyy")}";

                var offFileName = GetOffFileName(Db.DepartamentDef.Code, createDate);
                var directoryName = Path.GetDirectoryName(offFileName);
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);
                
                File.AppendAllLines(offFileName, new[] { lineForWrite });

                var paymentReasonName = PaymentReasons.FirstOrDefault(item => item.Id == reasonId)?.Name ?? string.Empty;
                InfoForCheck = new InfoForCheck(cost, createDate, Db.DepartamentDef.Code, Customer.Id, Customer.Name, paymentReasonName);
                return true;
            }
            catch (Exception e)
            {
                Log.ErrorWithException("Ошибка записи off файла", e);
                return false;
            }
        }

        private string GetOffFileName(string departamentCode, DateTime date)
        {
            string fileName = string.Format(Config.CustomerOutputFileFormat, departamentCode, date);
            return Path.Combine(Config.OutputPath, fileName);
        }
    }
}
