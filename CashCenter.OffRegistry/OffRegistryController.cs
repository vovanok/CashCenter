using CashCenter.Common;
using CashCenter.OffRegistry.Entities;
using System;
using System.IO;

namespace CashCenter.OffRegistry
{
    public class OffRegistryController
    {
        public void AddPayment(OffCustomerPayment customerPayment)
        {
            var offFileName = GetOffFileName(customerPayment.DepartmentCode, customerPayment.CreateDate);
            var lineForWrite = GetOffFileLine(customerPayment);
            var directoryName = Path.GetDirectoryName(offFileName);

            try
            {
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                File.AppendAllLines(offFileName, new[] { lineForWrite });
            }
            catch(Exception e)
            {
                Log.ErrorWithException("Ошибка записи информации о платеже в off файл.", e);
            }
        }
        
        private string GetOffFileLine(OffCustomerPayment customerPayment)
        {
            if (customerPayment == null)
                return string.Empty;

            var lineComponents = new[]
            {
                $"CustID = {customerPayment.CustomerNumber}",
                $"Day = {customerPayment.DayValue}",
                $"Night = {customerPayment.NightValue}",
                "",
                $"ReasonID = {customerPayment.ReasonId}",
                $"Total = {customerPayment.TotalCost}",
                $"id = {customerPayment.Id}",
                $"date = {customerPayment.CreateDate.ToString("dd.MM.yyyy")}"
            };

            return string.Join("; ", lineComponents);
        }

        private string GetOffFileName(string departamentCode, DateTime date)
        {
            string fileName = string.Format(Config.CustomerOutputFileFormat, departamentCode, date);
            return Path.Combine(Config.OutputDirectory, fileName);
        }
    }
}
