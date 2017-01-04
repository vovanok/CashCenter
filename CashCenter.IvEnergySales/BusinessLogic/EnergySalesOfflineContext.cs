using System;
using CashCenter.IvEnergySales.DbQualification;
using System.IO;
using CashCenter.IvEnergySales.Logging;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class EnergySalesOfflineContext : BaseEnergySalesContext
    {
        private const string OFF_FILE_NAME = "Payments.off";

        public EnergySalesOfflineContext(int customerId, DepartmentModel department, string dbCode)
            : base(customerId, department, dbCode)
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
                File.AppendAllLines(OFF_FILE_NAME, new[] { lineForWrite });
            }
            catch (Exception e)
            {
                Log.ErrorWithException("Ошибка записи off файла", e);
                return false;
            }

            return true;
        }
    }
}
