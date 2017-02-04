using CashCenter.Common;
using CashCenter.OffRegistry.Entities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashCenter.OffRegistry
{
    public class OffRegistryController
    {
        public void AddPayment(CustomerPayment customerPayment)
        {
            var offFileName = GetOffFileName(customerPayment.DepartmentCode, customerPayment.CreateDate);
            var lineComponents = new[]
            {
                $"CustID = {customerPayment.CustomerId}",
                $"Day = {customerPayment.DayValue}",
                $"Night = {customerPayment.NightValue}",
                "",
                $"ReasonID = {customerPayment.ReasonId}",
                $"Total = {customerPayment.TotalCost}",
                $"id = {customerPayment.Id}",
                $"date = {customerPayment.CreateDate.ToString("dd.MM.yyyy")}"
            };

            var lineForWrite = string.Join("; ", lineComponents);

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
        
        private string GetOffFileName(string departamentCode, DateTime date)
        {
            string fileName = string.Format(Config.CustomerOutputFileFormat, departamentCode, date);
            return Path.Combine(Config.OutputDirectory, fileName);
        }
    }
}
