using CashCenter.IvEnergySales.Utils;

namespace CashCenter.IvEnergySales.Check
{
	public class MainCheck : BaseCheck
	{
		public string RecipientName { get; set; }
		public string RecipientInn { get; set; }
		public string RecipientAddressLine1 { get; set; }
		public string RecipientAddressLine2 { get; set; }

		public string SellerName { get; set; }
		public string SellerInn { get; set; }
		public string SellerAddressLine1 { get; set; }
		public string SellerAddressLine2 { get; set; }

		public string CashierName { get; set; }

		public string PaymentName { get; set; }
		public decimal PaymentCost { get; set; }

		public string RecipientNameShort { get; set; } //Наволоцкий РЦ

		public MainCheck(CheckPrinter checkPrinter) : base(checkPrinter)
		{
		}

		protected override void CustomPrint()
		{
			printer.OpenCheck();

			printer.PrintLine("www.nalog.ru");
			printer.PrintLine($"Смена#{printer.SessionNumber}");
			printer.PrintLine(StringUtils.StringInCenter("КАССОВЫЙ ЧЕК", maxLineLength));

			printer.PrintLine(StringUtils.FilledString('*', maxLineLength));
			printer.PrintLine(RecipientName);
			printer.PrintLine($"ИНН {RecipientInn},");
			printer.PrintLine(RecipientAddressLine1);
			printer.PrintLine(RecipientAddressLine2);

			printer.PrintLine(StringUtils.FilledString('*', maxLineLength));
			printer.PrintLine(SellerName);
			printer.PrintLine($"ИНН {SellerInn},");
			printer.PrintLine("Адрес пункта приема платежей:");
			printer.PrintLine(SellerAddressLine1);
			printer.PrintLine(SellerAddressLine2);
			printer.PrintLine("Общая система налогообложения");
			printer.PrintLine("Бухгалтер-кассир:");
			printer.PrintLine(CashierName);

			printer.PrintLine(StringUtils.FilledString('*', maxLineLength));
			printer.PrintLine("ПРИХОД");
			printer.PrintLine("НАЛИЧНЫЕ ДЕНЕЖНЫЕ СРЕДСТВА");
			printer.PrintLine("Коммунальные платежи");
			printer.PrintLine(RecipientNameShort);

            printer.PrintLine("");

            printer.CheckType = 0;
            printer.Quantity = 1;
            printer.Price = PaymentCost;
            printer.Sale();

            printer.Summ1 = PaymentCost;
            printer.CloseCheck();
		}
	}
}
