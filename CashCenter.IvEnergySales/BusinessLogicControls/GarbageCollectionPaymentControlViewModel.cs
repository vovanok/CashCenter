using CashCenter.Check;
using CashCenter.Common;
using CashCenter.Common.Exceptions;
using CashCenter.IvEnergySales.BusinessLogic;
using CashCenter.IvEnergySales.Common;
using System;
using System.Windows;

namespace CashCenter.IvEnergySales.BusinessLogicControls
{
    public class GarbageCollectionPaymentControlViewModel : ViewModel
    {
        private GarbageCollectionPaymentContext context = new GarbageCollectionPaymentContext();

        public Observed<int> CustomerNumber { get; } = new Observed<int>();
        public Observed<int> RegionCode { get; } = new Observed<int>();
        public Observed<int> FinacialPeriod { get; } = new Observed<int>();
        public Observed<int> OrganizationCode { get; } = new Observed<int>();
        public Observed<int> FilialCode { get; } = new Observed<int>();
        public Observed<decimal> Cost { get; } = new Observed<decimal>();

        public Observed<bool> IsPaymentEnable { get; } = new Observed<bool>();

        public Command PayCommand { get; }
        public Command ClearCommand { get; }

        public GarbageCollectionPaymentControlViewModel()
        {
            CustomerNumber.OnChange += (newValue) => DispatchPropertyChanged("CustomerNumber");
            RegionCode.OnChange += (newValue) => DispatchPropertyChanged("RegionCode");
            FinacialPeriod.OnChange += (newValue) => DispatchPropertyChanged("FinacialPeriod");
            OrganizationCode.OnChange += (newValue) => DispatchPropertyChanged("OrganizationCode");
            FilialCode.OnChange += (newValue) => DispatchPropertyChanged("FilialCode");
            Cost.OnChange += (newValue) => DispatchPropertyChanged("Cost");

            IsPaymentEnable.OnChange += (newValue) => DispatchPropertyChanged("IsPaymentEnable");

            PayCommand = new Command(PayHandler);
            ClearCommand = new Command(ClearHandler);

            ClearHandler(null);
        }

        private void ApplyBarCode(string barCode)
        {
            // TODO: парсинг кода
            CustomerNumber.Value = 0;
            RegionCode.Value = 0;
            FinacialPeriod.Value = 0;
            OrganizationCode.Value = 0;
            FilialCode.Value = 0;
            Cost.Value = 0;
        }

        private void PayHandler(object parameters)
        {
            var controlForValidate = parameters as DependencyObject;
            if (!IsValid(controlForValidate))
            {
                Message.Error("При вводе были допущены ошибки. Исправьте их и попробуйте снова.\nОшибочные поля обведены красным.");
                return;
            }

            var isWithoutCheck = false;
            if (!CheckPrinter.IsReady)
            {
                if (!Message.YesNoQuestion("Кассовый аппарат не подключен. Продолжить без печати чека?"))
                    return;

                isWithoutCheck = true;
            }

            var operationName = "Оплата за вывоз ТКО";
            try
            {
                Log.Info($"Старт -> {operationName}");

                using (new OperationWaiter())
                {
                    context.Pay(FinacialPeriod.Value, DateTime.Now, OrganizationCode.Value,
                        FilialCode.Value, CustomerNumber.Value, Cost.Value, isWithoutCheck);
                    ClearHandler(null);
                }

                Log.Info($"Успешно -> {operationName}");
            }
            catch (IncorrectDataException ex)
            {
                Message.Error(ex.Message);
            }
            catch (Exception ex)
            {
                Message.Error($"Ошибка выполнения операция оплаты за воду");
                Log.Error($"Ошибка -> {operationName}", ex);
            }
        }

        private void ClearHandler(object parameters)
        {
            CustomerNumber.Value = 0;
            RegionCode.Value = 0;
            FinacialPeriod.Value = 0;
            OrganizationCode.Value = 0;
            FilialCode.Value = 0;
            Cost.Value = 0;

            IsPaymentEnable.Value = false;
        }
    }
}
