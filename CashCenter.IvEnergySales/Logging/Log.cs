namespace CashCenter.IvEnergySales.Logging
{
	public abstract class Log
	{
		protected enum LogMessageType
		{
			Info,
			Warning,
			Error
		}

		private static Log instance;

		private static Log Instance
		{
			get
			{
				if (instance == null)
					instance = new MessageBoxLog(); // TODO: сделать выбор

				return instance;
			}
		}

		public static void Info(string message)
		{
			Instance.InfoLog(message);
		}

		public static void Warning(string message)
		{
			Instance.WarningLog(message);
		}

		public static void Error(string message)
		{
			Instance.ErrorLog(message);
		}

		protected abstract void InfoLog(string message);
		protected abstract void WarningLog(string message);
		protected abstract void ErrorLog(string message);
	}
}
