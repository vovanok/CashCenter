using System;
using System.IO;
using DrvFRLib;
using CashCenter.Common;


namespace CashCenter.Check
{
    public class CashMachine
    {
        public DrvFR Driver { get; private set; }

        public CashMachine()
        {
            try
            {
                LogInfo("Создание драйвера");
                Driver = new DrvFR();
                Driver.Timeout = 4000;
                Driver.Password = Config.CheckPrinterPassword;
                Driver.LogOn = true;
                Driver.LogFileMaxSize = 100;
                Driver.LogMaxFileCount = 10;
                Driver.LogCommands = true;
                Driver.LogMethods = true;
                Driver.ComLogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ComLog.txt");
                Driver.CPLogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CPLog.txt");
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
        }

        public bool IsReady
        {
            get
            {
                try
                {
                    LogInfo("Проверка готовности");
                    PrintLine(string.Empty);
                    return true;
                }
                catch (Exception ex)
                {
                    LogError($"ККМ не готов ({ex.Message}).");
                    return false;
                }
            }
        }

        public bool IsSessionOpen
        {
            get
            {
                if (Driver == null)
                    return false;

                Driver.GetECRStatus();
                return Driver.ECRMode != 4;
            }
        }

        public void Connect()
        {
            LogInfo("Соединение с ККМ");
            Driver?.Connect();
            CheckError();
        }

        public void Disconnect()
        {
            LogInfo("Разрыв соединения с ККМ");
            Driver?.Disconnect();
            CheckError();
        }

        public void OpenSessionIfNot()
        {
            LogInfo("Открытие сессии");

            if (Driver == null)
                return;

            if (!IsSessionOpen)
            {
                // синхронизируем  время
                Driver.Time = DateTime.Now;
                Driver.SetTime();

                // синхронизируем дату
                Driver.Date = DateTime.Now.Date;
                Driver.SetDate();
                Driver.ConfirmDate();

                Driver.OpenSession();
            }

            CheckError();
        }

        public void SendEmail(string email)
        {
            LogInfo($"Отправка чека на email {email}");

            if (Driver == null)
                return;

            Driver.CustomerEmail = email;
            Driver.FNSendCustomerEmail();
            CheckError();
        }

        public void CloseSession()
        {
            LogInfo("Закрытие сессии");
            Driver?.FNCloseSession();
            CheckError();
        }

        public void PrintLine(string line)
		{
            LogInfo($"Печать строки: {line}");

			if (Driver == null)
				return;

			Driver.StringForPrinting = line;
			Driver.PrintString();

			CheckError();
		}

		public void OpenCheck()
		{
            LogInfo("Открытие чека");
			Driver?.OpenCheck();
			CheckError();
		}

		public void CloseCheck()
		{
            LogInfo("Закрытие чека");
			Driver?.CloseCheck();
			CheckError();
		}

		public void Cut()
		{
            LogInfo("Обрезание чека");

			if (Driver == null)
				return;

			Driver.CutType = true;
			Driver.CutCheck();

			CheckError();
		}

		public void ShowProperties()
		{
            LogInfo("Открытие окна свойств ККМ");
			Driver?.ShowProperties();
		}

        public void Sale()
        {
            LogInfo("Продажа");
            Driver?.Sale();
            CheckError();
        }

        public void CancelCheck()
        {
            LogInfo("Отмена чека");
            Driver?.CancelCheck();
            CheckError();
        }

        public void SysAdminCancelCheck()
        {
            LogInfo("Административная отмена чека");
            Driver?.SysAdminCancelCheck();
            CheckError();
        }

        private void CheckError()
		{
            if (Driver == null)
                return;

            if (Driver.ResultCode != 0)
            {
                throw new CheckException(Driver.ResultCode, Driver.ResultCodeDescription);
            }
		}

        private void LogInfo(string message)
        {
            Log.Info($"ККМ: {message}");
        }

        private void LogError(string message)
        {
            Log.Error($"Ошибка ККМ: {message}");
        }
	}
}