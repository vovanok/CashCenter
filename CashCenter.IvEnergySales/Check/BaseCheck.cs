using System;
using System.Text;
using CashCenter.IvEnergySales.Logging;

namespace CashCenter.IvEnergySales.Check
{
	public abstract class BaseCheck
	{
		private const int MIN_LINE_LENGTH = 10;
		
		protected int maxLineLength;
		protected CheckPrinter printer;

		protected BaseCheck(CheckPrinter checkPrinter)
		{
			printer = checkPrinter;
			maxLineLength = Math.Max(Config.CheckPrinterMaxLineLength, MIN_LINE_LENGTH);
		}

		public bool Print()
		{
			printer.ResetErrors();

			CustomPrint();

			if (printer.ErrorsOperationsResults.Count > 0)
			{
				var sbErrorMessage = new StringBuilder();
				foreach (var errorOerationResult in printer.ErrorsOperationsResults)
					sbErrorMessage.AppendLine($"{errorOerationResult.Description} ({errorOerationResult.ErrorCode})");

				Log.Error($"Ошибка печати чека.\n{sbErrorMessage.ToString()}");
				return false;
			}

			return true;
		}

		protected abstract void CustomPrint();
	}
}
