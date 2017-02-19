using CashCenter.Common;

namespace CashCenter.IvEnergySales.Check
{
	public class CustomerCheck : BaseCheck
	{
		public string SalesDepartmentInfo { get; set; }
        public string DepartmentCode { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PaymentReason { get; set; }
        public string CashierName { get; set; }

        public decimal Cost { get; set; }

		public CustomerCheck(CheckPrinter checkPrinter) : base(checkPrinter)
		{
		}

		protected override void CustomPrint()
		{
            printer.OpenCheck();

            printer.PrintLine(StringUtils.StringInCenter("КАССОВЫЙ ЧЕК", Config.CheckPrinterMaxLineLength));
            printer.PrintLine(StringUtils.FilledString('*', Config.CheckPrinterMaxLineLength));
            printer.PrintText(Config.SalesMainInfo);
            printer.PrintLine(StringUtils.FilledString('*', Config.CheckPrinterMaxLineLength));

            printer.PrintText(SalesDepartmentInfo);
            printer.PrintLine(StringUtils.FilledString('*', Config.CheckPrinterMaxLineLength));

            printer.PrintText($"Код отделения: {DepartmentCode}");
            printer.PrintText($"Лицевой счет: {CustomerId}");
            printer.PrintText($"ФИО: {CustomerName}");
            printer.PrintText($"Основание: {PaymentReason.ToUpper()}");
            printer.PrintText($"Кассир: {CashierName}");

            printer.PrintLine("");

            printer.CheckType = 0;
            printer.Quantity = 1;
            printer.Price = Cost;
            printer.Department = 1;

            printer.Tax1 = Config.NdsPercent == 18
                ? 1
                : Config.NdsPercent == 10
                    ? 2
                    : Config.NdsPercent == 20
                        ? 3
                        : 0;
            printer.Tax2 = 0;
            printer.Tax3 = 0;
            printer.Tax4 = 0;

            printer.Sale();

            printer.Summ1 = Cost;
            printer.CloseCheck();
		}
	}
}
