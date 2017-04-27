using DrvFRLib;
using System;
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
                Driver.Timeout = 1000;
            }
            catch (Exception ex)
            {
                throw new SystemException(ex.Message);
            }
			
			Driver.Password = Config.CheckPrinterPassword;
            OpenSession();
		}

        ~CashMachine()
        {
            CloseSession(); // TODO: move it from destructor
        }

        public bool IsReady // TODO: find better way
        {
            get
            {
                try
                {
                    LogInfo("Проверка готовности");
                    PrintLine(string.Empty);
                    return true;
                }
                catch(Exception)
                {
                    return false;
                }
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

        private void OpenSession()
        {
            Logger.Info("Открытие сессии");

            if (Driver == null)
                return;

            Driver.GetECRStatus();
            if (Driver.ECRMode == 4) //Смена закрыта, открываем новую
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

        private void CloseSession()
        {
            LogInfo("Закрытие сессии");
            Driver?.Disconnect(); // TODO: seems like error
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
            Logger.Info($"ККМ: {message}");
        }

        private void LogError(string message)
        {
            Logger.Error($"Ошибка ККМ: {message}");
        }
	}
}
