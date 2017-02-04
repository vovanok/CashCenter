using System;

namespace CashCenter.Common
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

        public static void SetLogger(Log logger)
        {
            instance = logger;
        }

		public static void Info(string message)
		{
			instance?.InfoLog(message);
		}

		public static void Warning(string message)
		{
			instance?.WarningLog(message);
		}

		public static void Error(string message)
		{
			instance?.ErrorLog(message);
		}

        public static void ErrorWithException(string headerMessage, Exception exception)
        {
            instance?.ErrorLog($"{headerMessage}\n\nИсключение: {exception.Message}\n\nТрассировка стека:\n{exception.StackTrace}");
        }

		protected abstract void InfoLog(string message);
		protected abstract void WarningLog(string message);
		protected abstract void ErrorLog(string message);
	}
}
