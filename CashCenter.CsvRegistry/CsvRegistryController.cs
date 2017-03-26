using CashCenter.Common;
using CashCenter.CsvRegistry.Entities;
using System;
using System.IO;

namespace CashCenter.CsvRegistry
{
    public class CsvRegistryController
    {
        public void AddPayment(CsvOrganizationPayment organizationPayment)
        {
            var csvFileName = GetCsvFileName(organizationPayment.DepartmentCode, organizationPayment.CreateDate);
            var lineForWrite =
                string.Join(";", new object[] {
                    organizationPayment.DepartmentCode,
                    organizationPayment.OrganizationId,
                    organizationPayment.CreateDate.ToString("dd.MM.yyyy"),
                    organizationPayment.DocumentNumber,
                    organizationPayment.CreateDate.ToString("dd.MM.yyyy"),
                    organizationPayment.Comment,
                    organizationPayment.Cost.ToString("0.00"),
                    organizationPayment.PaymentTypeId,
                    organizationPayment.PaymentTypeName,
                    organizationPayment.Code1C,
                    organizationPayment.IncastCodePayment,
                    organizationPayment.ReasonId
                });

            var directoryName = Path.GetDirectoryName(csvFileName);

            try
            {
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                File.AppendAllLines(csvFileName, new[] { lineForWrite });
            }
            catch (Exception e)
            {
                Log.ErrorWithException("Ошибка записи информации о платеже в csv файл.", e);
            }
        }

        private string GetCsvFileName(string departamentCode, DateTime date)
        {
            string fileName = string.Format(Config.OrganizationOutputFileFormat, departamentCode, date);
            return Path.Combine(Config.OutputDirectory, fileName);
        }
    }
}
