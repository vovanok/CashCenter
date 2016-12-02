using System.Collections.Generic;
using System.Windows;

namespace CashCenter.IvEnergySales.Logging
{
	public class MessageBoxLog : Log
	{
		private readonly Dictionary<LogMessageType, string> logsCaptions =
			new Dictionary<LogMessageType, string>
			{
				{ LogMessageType.Info, "Info" },
				{ LogMessageType.Warning, "Warning" },
				{ LogMessageType.Error, "Error" }
			};

		private readonly Dictionary<LogMessageType, MessageBoxImage> logsImages =
			new Dictionary<LogMessageType, MessageBoxImage>
			{
				{ LogMessageType.Info, MessageBoxImage.Information },
				{ LogMessageType.Warning, MessageBoxImage.Warning },
				{ LogMessageType.Error, MessageBoxImage.Error }
			};

		private void Log(string message, LogMessageType messageType)
		{
			MessageBox.Show(message, GetCaptionByLogType(messageType), MessageBoxButton.OK, GetMessageBoxImageByLogType(messageType));
		}

		private string GetCaptionByLogType(LogMessageType logMessageType)
		{
			return logsCaptions.ContainsKey(logMessageType) ? logsCaptions[logMessageType] : "Message";
		}

		private MessageBoxImage GetMessageBoxImageByLogType(LogMessageType logMessageType)
		{
			return logsImages.ContainsKey(logMessageType) ? logsImages[logMessageType] : MessageBoxImage.None;
		}

		protected override void InfoLog(string message)
		{
			Log(message, LogMessageType.Info);
		}

		protected override void WarningLog(string message)
		{
			Log(message, LogMessageType.Warning);
		}

		protected override void ErrorLog(string message)
		{
			Log(message, LogMessageType.Error);
		}
	}
}
