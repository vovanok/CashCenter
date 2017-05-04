using System.Collections.Generic;
using DrvFRLib;
using System;
using CashCenter.Common;

namespace CashCenter.IvEnergySales.Check
{
	public class CashMachine
    {
		public class OperationResult
		{
			public int ErrorCode { get; private set; }

			public string Description { get; private set; }

			public bool IsSuccess
			{
				get { return ErrorCode == 0; }
			}

			internal OperationResult(int errorCode, string description)
			{
				ErrorCode = errorCode;
				Description = description;
			}
		}

		public DrvFR Driver { get; private set; }

		public List<OperationResult> Errors { get; private set; } 

		public CashMachine()
		{
            try
            {
                Driver = new DrvFR();
                Driver.Timeout = 1000;
            }
            catch (Exception e)
            {
                Log.ErrorWithException("Ошибка создания драйвера кассового аппарата.", e);
                throw;
            }
			
			Driver.Password = Config.CheckPrinterPassword;
            Driver.OpenSession();
			Errors = new List<OperationResult>();
		}

        ~CashMachine()
        {
            CloseSession(); // TODO: move it from destructor
        }

        public bool IsReady // TODO: find better way
        {
            get
            {
                int countErrorsBeforeTest = Errors.Count;
                PrintLine(string.Empty);
                int countErrorsAfterTest = Errors.Count;
                return countErrorsAfterTest <= countErrorsBeforeTest;
            }
        }

        public void Connect()
        {
            Driver?.Connect();
            CheckError();
        }

        public void Disconnect()
        {
            Driver?.Disconnect();
            CheckError();
        }

        private void OpenSession()
        {
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
            Driver?.Disconnect(); // TODO: seems like error

            CheckError();
        }

        public void PrintLine(string line)
		{
			if (Driver == null)
				return;

			Driver.StringForPrinting = line;
			Driver.PrintString();

			CheckError();
		}

		public void OpenCheck()
		{
			Driver?.OpenCheck();
			CheckError();
		}

		public void CloseCheck()
		{
			Driver?.CloseCheck();
			CheckError();
		}

		public void Cut()
		{
			if (Driver == null)
				return;

			Driver.CutType = true;
			Driver.CutCheck();

			CheckError();
		}

		public void ShowProperties()
		{
			Driver?.ShowProperties();
		}

		public void ResetErrors()
		{
			Errors.Clear();
		}

        public void Sale()
        {
            Driver?.Sale();
            CheckError();
        }

        public void CancelCheck()
        {
            Driver?.CancelCheck();
            CheckError();
        }

        private void CheckError()
		{
            if (Driver == null)
                return;

			if (Errors == null)
				Errors = new List<OperationResult>();

			if (Driver.ResultCode != 0)
				Errors.Add(new OperationResult(Driver.ResultCode, Driver.ResultCodeDescription));
		}
	}
}
