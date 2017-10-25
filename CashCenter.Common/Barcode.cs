using CashCenter.Common.Exceptions;
using System;
using System.Collections.Generic;

namespace CashCenter.Common
{
    public class Barcode
    {
        public int FinancialPeriod { get; private set; }
        public int RegionCode { get; private set; }
        public int CustomerNumber { get; private set; }
        public int CostInKopeck { get; private set; }
        public int OrganizationCode { get; private set; }

        private Barcode()
        {
        }

        public static Barcode Parse(string barcodeString)
        {
            if (string.IsNullOrEmpty(barcodeString) || barcodeString.Length != 30)
                throw new ArgumentException("Штрих код не задан или имеет неверный формат");

            var errors = new List<string>();

            string financialPeriodStr = barcodeString.Substring(0, 3);
            if (!int.TryParse(financialPeriodStr, out int financialPeriod))
                errors.Add($"Код финансового периода не корректен ({financialPeriodStr})");

            string regionCodeStr = barcodeString.Substring(5, 2);
            if (!int.TryParse(regionCodeStr, out int regionCode))
                errors.Add($"Код региона не корректен ({regionCodeStr})");

            string customerNumberStr = barcodeString.Substring(7, 10);
            if (!int.TryParse(customerNumberStr, out int customerNumber))
                errors.Add($"Лицевой счет не корректен ({customerNumberStr})");

            string costInKopeckStr = barcodeString.Substring(17, 9);
            if (!int.TryParse(costInKopeckStr, out int costInKopeck))
                errors.Add($"Сумма не корректна ({costInKopeckStr})");

            string organizationCodeStr = barcodeString.Substring(26, 4);
            if (!int.TryParse(organizationCodeStr, out int organizationCode))
                errors.Add($"Код организации не корректен ({organizationCodeStr})");

            if (errors.Count > 0)
                throw new IncorrectDataException(errors);

            return new Barcode
            {
                FinancialPeriod = financialPeriod,
                RegionCode = regionCode,
                CustomerNumber = customerNumber,
                CostInKopeck = costInKopeck,
                OrganizationCode = organizationCode
            };
        }
    }
}
