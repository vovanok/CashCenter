namespace CashCenter.Objective.HotWater
{
    public class TxtHotWaterCustomer
    {
        // Лицевой счет
        public int Number { get; private set; }

        // ФИО потребителя
        public string Name { get; private set; }

        // Адрес потребителя
        public string Address { get; private set; }

        // Период начисления
        public string TimePeriodCode { get; private set; }

        // Сумма к оплате
        public decimal TotalForPay { get; private set; }

        // Наименование счетчика №1
        public string CounterName1 { get; private set; }

        // Показания счетчика №1
        public double CounterValue1 { get; private set; }

        // Наименование счетчика №2
        public string CounterName2 { get; private set; }

        // Показания счетчика №2
        public double CounterValue2 { get; private set; }

        // Наименование счетчика №3
        public string CounterName3 { get; private set; }

        // Показания счетчика №3
        public double CounterValue3 { get; private set; }

        // Наименование счетчика №4
        public string CounterName4 { get; private set; }

        // Показания счетчика №4
        public double CounterValue4 { get; private set; }

        // Наименование счетчика №5
        public string CounterName5 { get; private set; }

        // Показания счетчика №5
        public double CounterValue5 { get; private set; }

        public TxtHotWaterCustomer(
            int number, string name, string address,
            string timePeriodCode, decimal totalForPay,
            string counterName1, double counterValue1,
            string counterName2, double counterValue2,
            string counterName3, double counterValue3,
            string counterName4, double counterValue4,
            string counterName5, double counterValue5)
        {
            Number = number;
            Name = name;
            Address = address;
            TimePeriodCode = timePeriodCode;
            TotalForPay = totalForPay;
            CounterName1 = counterName1;
            CounterValue1 = counterValue1;
            CounterName2 = counterName2;
            CounterValue2 = counterValue2;
            CounterName3 = counterName3;
            CounterValue3 = counterValue3;
            CounterName4 = counterName4;
            CounterValue4 = counterValue4;
            CounterName5 = counterName5;
            CounterValue5 = counterValue5;
        }
    }
}
