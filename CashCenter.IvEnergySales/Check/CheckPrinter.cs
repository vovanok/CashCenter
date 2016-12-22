using System.Collections.Generic;
using DrvFRLib;
using System;
using CashCenter.IvEnergySales.Logging;

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

		private DrvFR cashHardwareDriver;

		public List<OperationResult> ErrorsOperationsResults { get; private set; } 

		public CheckPrinter()
		{
            try
            {
                cashHardwareDriver = new DrvFR();
            }
            catch (Exception e)
            {
                Log.ErrorWithException("Ошибка создания драйвера кассаового аппарата.", e);
                throw;
            }
			
			cashHardwareDriver.Password = Config.CheckPrinterPassword;
			ErrorsOperationsResults = new List<OperationResult>();
		}

		public int SessionNumber
		{
			get
			{
				if (cashHardwareDriver == null)
					return 0;

				return cashHardwareDriver.SessionNumber;
			}
		}

		public decimal Price
        {
            get { return cashHardwareDriver != null ? cashHardwareDriver.Price : 0; }
            set
            {
                if (cashHardwareDriver == null)
                    return;

                cashHardwareDriver.Price = value;
            }
        }

        public double Quantity
        {
            get { return cashHardwareDriver != null ? cashHardwareDriver.Quantity : 0; }
            set
            {
                if (cashHardwareDriver == null)
                    return; cashHardwareDriver.Quantity = value;
            }
        }

        public decimal Summ1
        {
            get { return cashHardwareDriver != null ? cashHardwareDriver.Summ1 : 0; }
            set
            {
                if (cashHardwareDriver == null)
                    return; cashHardwareDriver.Summ1 = value;
            }
        }

        public int CheckType
        {
            get { return cashHardwareDriver != null ? cashHardwareDriver.CheckType : 0; }
            set
            {
                if (cashHardwareDriver == null)
                    return; cashHardwareDriver.CheckType = value;
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

		public void PrintEmptyLine()
		{
			PrintLine(" ");
		}

		public void PrintLine(string line)
		{
			if (cashHardwareDriver == null)
				return;

			cashHardwareDriver.StringForPrinting = line;
			cashHardwareDriver.PrintString();

			CheckError();
		}

		public void OpenCheck()
		{
			if (cashHardwareDriver == null)
				return;

			cashHardwareDriver.OpenCheck();

			CheckError();
		}

		public void CloseCheck()
		{
			if (cashHardwareDriver == null)
				return;

			cashHardwareDriver.CloseCheck();

			CheckError();
		}

		public void Cut()
		{
			if (cashHardwareDriver == null)
				return;

			cashHardwareDriver.CutType = true;
			cashHardwareDriver.FeedAfterCut = true;
			cashHardwareDriver.FeedLineCount = 1;
			cashHardwareDriver.CutCheck();

			CheckError();
		}

		public void ShowProperties()
		{
			cashHardwareDriver?.ShowProperties();
		}

		public void ResetErrors()
		{
			ErrorsOperationsResults.Clear();
		}

        public void Sale()
        {
            if (cashHardwareDriver == null)
                return;

            cashHardwareDriver.Sale();

            CheckError();
        }

        public void CancelCheck()
        {
            if (cashHardwareDriver == null)
                return;

            cashHardwareDriver.CancelCheck();

            CheckError();
        }

        private void CheckError()
		{
			if (ErrorsOperationsResults == null)
				ErrorsOperationsResults = new List<OperationResult>();

			if (cashHardwareDriver.ResultCode != 0)
				ErrorsOperationsResults.Add(new OperationResult(cashHardwareDriver.ResultCode, cashHardwareDriver.ResultCodeDescription));
		}
	}
}
