using CashCenter.Common;
using CashCenter.DataMigration.Providers.Rkc.Entities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace CashCenter.DataMigration.Providers.Rkc
{
    internal class RkcController
    {
        public void StoreItems(IEnumerable<RkcAllPaymentsItem> items)
        {
            if (items == null)
                return;

            var directoryName = Config.OutputDirectory;

            try
            {
                // Create Output directory if not exist
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                var sbRkcContent = new StringBuilder();
                sbRkcContent.AppendLine("Ver|O_RKC_1C_1|>>");
                foreach (var item in items)
                {
                    sbRkcContent.Append(string.Join("|",
                        new[]
                        {
                            ((int)item.Type).ToString(),
                            item.OrganizationCode.ToString("D3"),
                            ((int)item.Target).ToString("D3"),
                            item.Inn ?? string.Empty,
                            item.Kpp ?? string.Empty,
                            item.DepartmentCode ?? string.Empty,
                            item.PaymentDate.ToString("dd.MM.yyyy"),
                            item.PaymentTotal.ToString("0.00", CultureInfo.InvariantCulture),
                            item.PaymentCommission.ToString("0.00", CultureInfo.InvariantCulture)
                        }));

                    sbRkcContent.AppendLine("|>>");
                }

                var rkcFilename = Path.Combine(directoryName, string.Format(Config.AllPaymentsRfcOutputFileFormat, DateTime.Now));

                if (File.Exists(rkcFilename))
                    File.Delete(rkcFilename);

                File.AppendAllText(rkcFilename, sbRkcContent.ToString(), Encoding.ASCII);
            }
            catch (Exception ex)
            {
                throw new SystemException("Ошибка записи информации о платеже в off файл", ex);
            }
        }
    }
}
