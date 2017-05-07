using CashCenter.Common;
using System.Collections.Generic;

namespace CashCenter.IvEnergySales.Check
{
    public class CustomerCheck : CashCenter.Check.Check
    {
        public string SalesDepartmentInfo { get; private set; }
        public string DepartmentCode { get; private set; }
        public int CustomerId { get; private set; }
        public string CustomerName { get; private set; }
        public string PaymentReason { get; private set; }
        public string CashierName { get; private set; }

        public CustomerCheck(string salesDepartmentInfo, string departmentCode,
                int customerNumber, string customerName, string paymentReason, string cashierName, decimal cost)
            : base(new List<string>(), cost)
        {
            CommonLines.Add(StringUtils.StringInCenter("КАССОВЫЙ ЧЕК", Config.CheckPrinterMaxLineLength));
            CommonLines.Add(StringUtils.FilledString('*', Config.CheckPrinterMaxLineLength));
            CommonLines.AddRange(StringUtils.SplitStringWithSeparators(Config.SalesMainInfo));
            CommonLines.Add(StringUtils.FilledString('*', Config.CheckPrinterMaxLineLength));
            CommonLines.AddRange(StringUtils.SplitStringWithSeparators(salesDepartmentInfo));
            CommonLines.Add(StringUtils.FilledString('*', Config.CheckPrinterMaxLineLength));
            CommonLines.Add($"Код отделения: {departmentCode}");
            CommonLines.Add($"Лицевой счет: {customerNumber}");
            CommonLines.Add($"ФИО: {customerName}");
            CommonLines.Add($"Основание: {paymentReason.ToUpper()}");
            CommonLines.Add($"Кассир: {cashierName}");
            CommonLines.Add("ПРИХОД");
            CommonLines.Add("НАЛИЧНЫЕ ДЕНЕЖНЫЕ СРЕДСТВА");
            CommonLines.Add("ЭЛЕКТРОЭНЕРГИЯ");
        }
    }
}
