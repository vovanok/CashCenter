using System.Collections.Generic;
using DrvFRLib;
using System;
using CashCenter.Common;

namespace CashCenter.IvEnergySales.Check
{
	public class CheckPrinter
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

		private DrvFR driver;

		public List<OperationResult> ErrorsOperationsResults { get; private set; } 

		public CheckPrinter()
		{
            try
            {
                driver = new DrvFR();
            }
            catch (Exception e)
            {
                Log.ErrorWithException("Ошибка создания драйвера кассаового аппарата.", e);
                throw;
            }
			
			driver.Password = Config.CheckPrinterPassword;
            driver.OpenSession();
			ErrorsOperationsResults = new List<OperationResult>();
		}

		public int SessionNumber
		{
			get
			{
				if (driver == null)
					return 0;

				return driver.SessionNumber;
			}
		}

		public decimal Price
        {
            get { return driver != null ? driver.Price : 0; }
            set
            {
                if (driver == null)
                    return;

                driver.Price = value;
            }
        }

        public double Quantity
        {
            get { return driver != null ? driver.Quantity : 0; }
            set
            {
                if (driver == null)
                    return; driver.Quantity = value;
            }
        }

        public decimal Summ1
        {
            get { return driver != null ? driver.Summ1 : 0; }
            set
            {
                if (driver == null)
                    return; driver.Summ1 = value;
            }
        }

        public int CheckType
        {
            get { return driver != null ? driver.CheckType : 0; }
            set
            {
                if (driver == null)
                    return; driver.CheckType = value;
            }
        }

        public bool IsReady
        {
            get
            {
                int countErrorsBeforeTest = ErrorsOperationsResults.Count;
                PrintLine(string.Empty);
                int countErrorsAfterTest = ErrorsOperationsResults.Count;
                return countErrorsAfterTest <= countErrorsBeforeTest;
            }
        }

        public int Department
        {
            get { return driver != null ? driver.Department : 0; }
            set
            {
                if (driver != null)
                    driver.Department = value;
            }
        }

        public int Tax1
        {
            get { return driver != null ? driver.Tax1 : 0; }
            set
            {
                if (driver != null)
                    driver.Tax1 = value;
            }
        }

        public int Tax2
        {
            get { return driver != null ? driver.Tax2 : 0; }
            set
            {
                if (driver != null)
                    driver.Tax2 = value;
            }
        }

        public int Tax3
        {
            get { return driver != null ? driver.Tax3 : 0; }
            set
            {
                if (driver != null)
                    driver.Tax3 = value;
            }
        }

        public int Tax4
        {
            get { return driver != null ? driver.Tax4 : 0; }
            set
            {
                if (driver != null)
                    driver.Tax4 = value;
            }
        }

        public void Connect()
        {
            driver?.Connect();

            CheckError();
        }

        public void Disconnect()
        {
            driver?.Disconnect();

            CheckError();
        }

        private void OpenSession()
        {
            if (driver == null)
                return;

            driver.GetECRStatus();
            if (driver.ECRMode == 4) //Смена закрыта, открываем новую
            {
                // синхронизируем  время
                driver.Time = DateTime.Now;
                driver.SetTime();

                // синхронизируем дату
                driver.Date = DateTime.Now.Date;
                driver.SetDate();
                driver.ConfirmDate();

                driver.OpenSession();
            }

            CheckError();
        }

        private void CloseSession()
        {
            if (driver == null)
                return;

            driver.Disconnect();

            CheckError();
        }

		public void PrintEmptyLine()
		{
			PrintLine(" ");
		}

        public void PrintText(string text)
        {
            var lines = text.Split('|');
            foreach(var line in lines)
            {
                var textLines = StringUtils.SplitStringByLines(line, Config.CheckPrinterMaxLineLength);
                foreach (string textLine in textLines)
                    PrintLine(textLine);
            }
        }

		public void PrintLine(string line)
		{
			if (driver == null)
				return;

			driver.StringForPrinting = line;
			driver.PrintString();

			CheckError();
		}

		public void OpenCheck()
		{
			if (driver == null)
				return;

			driver.OpenCheck();

			CheckError();
		}

		public void CloseCheck()
		{
			if (driver == null)
				return;

			driver.CloseCheck();

			CheckError();
		}

		public void Cut()
		{
			if (driver == null)
				return;

			driver.CutType = true;
			driver.CutCheck();

			CheckError();
		}

		public void ShowProperties()
		{
			driver?.ShowProperties();
		}

		public void ResetErrors()
		{
			ErrorsOperationsResults.Clear();
		}

        public void Sale()
        {
            if (driver == null)
                return;

            driver.Sale();

            CheckError();
        }

        public void CancelCheck()
        {
            if (driver == null)
                return;

            driver.CancelCheck();

            CheckError();
        }

        private void CheckError()
		{
			if (ErrorsOperationsResults == null)
				ErrorsOperationsResults = new List<OperationResult>();

			if (driver.ResultCode != 0)
				ErrorsOperationsResults.Add(new OperationResult(driver.ResultCode, driver.ResultCodeDescription));
		}

        ~CheckPrinter()
        {
            CloseSession();
        }
	}
}
