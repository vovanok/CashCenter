using CashCenter.IvEnergySales.Utils;

namespace CashCenter.IvEnergySales.Check
{
	public class MainCheck : BaseCheck
	{
		public string SalesDepartmentInfo { get; set; }
        public string DepartmentCode { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PaymentReason { get; set; }
        public string CashierName { get; set; }

        public decimal Cost { get; set; }

		public MainCheck(CheckPrinter checkPrinter) : base(checkPrinter)
		{
		}

		protected override void CustomPrint()
		{
            printer.OpenCheck();

            printer.PrintText(SalesDepartmentInfo);
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

            printer.Sale();

            printer.Summ1 = Cost;
            printer.CloseCheck();
		}
	}
}
