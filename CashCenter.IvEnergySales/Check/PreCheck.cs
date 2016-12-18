using System;
using CashCenter.IvEnergySales.Utils;

namespace CashCenter.IvEnergySales.Check
{
	public class PreCheck : BaseCheck
	{
		public DateTime Date { get; set; }
		public string RecipientNameShort { get; set; }
		public decimal Cost { get; set; }

		public PreCheck(CheckPrinter checkPrinter)
			: base(checkPrinter)
		{
		}

		protected override void CustomPrint()
		{
			printer.PrintLine(StringUtils.StringInCenter("ООО \"Ивановоэнергосбыт\"", maxLineLength));
			printer.PrintLine(StringUtils.StringInCenter("ДОБРО ПОЖАЛОВАТЬ !", maxLineLength));
			printer.PrintLine($"Дата: {Date.ToString("dd.MM.yyyy HH:mm:ss")}");
			printer.PrintLine("Заявка на платеж");
			printer.PrintLine("Оплата:");
			printer.PrintLine(RecipientNameShort ?? string.Empty);
			printer.PrintLine(StringUtils.FilledLeftFromContentString(Cost.ToString("0.00"), '.', maxLineLength));
			printer.PrintEmptyLine();
			printer.PrintLine("Данные указаны верно");
			printer.PrintEmptyLine();
			printer.PrintLine(StringUtils.FilledString('-', maxLineLength));
			printer.PrintLine("Подпись плательщика");
			for (int i = 0; i < Config.CheckPrinterCountEmptyLinesAfterCheck; i++)
				printer.PrintEmptyLine();

			printer.Cut();
		}
	}
}
