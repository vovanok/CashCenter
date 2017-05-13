using CashCenter.Common;
using CashCenter.DataMigration.Providers.Off.Entities;
using System;
using System.Collections.Generic;
using System.IO;

namespace CashCenter.DataMigration.Providers.Off
{
    public class OffRegistryController
    {
        public void StorePayments(IEnumerable<OffCustomerPayment> customerPayments)
        {
            if (customerPayments == null)
                return;

            var directoryName = Config.OutputDirectory;

            try
            {
                // Create Output directory if not exist
                if (!Directory.Exists(directoryName))
                    Directory.CreateDirectory(directoryName);

                // Key - filename for store, Value - lines for record to file
                var filenameToContentMap = new Dictionary<string, IList<string>>();

                foreach (var customerPayment in customerPayments)
                {
                    var offFilename = GetOffFileName(customerPayment.DepartmentCode, customerPayment.CreateDate);
                    var offFilepath = Path.Combine(Config.OutputDirectory, offFilename);

                    if (!filenameToContentMap.ContainsKey(offFilepath))
                        filenameToContentMap.Add(offFilepath, new List<string>());

                    filenameToContentMap[offFilepath].Add(customerPayment.ToOffFileLine());
                }

                foreach (var filenameContentPair in filenameToContentMap)
                {
                    if (File.Exists(filenameContentPair.Key))
                        File.Delete(filenameContentPair.Key);

                    File.AppendAllLines(filenameContentPair.Key, filenameContentPair.Value);
                }
            }
            catch(Exception ex)
            {
                throw new SystemException("Ошибка записи информации о платеже в off файл", ex);
            }
        }

        private string GetOffFileName(string departmentCode, DateTime date)
        {
            return string.Format(Config.EnergyCustomerOffOutputFileFormat, departmentCode, date);
        }
    }
}
