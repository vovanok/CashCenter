using System;

namespace CashCenter.IvEnergySales.Exceptions
{
    public class CustomerNotAppliedException : UserException
    {
        public override string Message => "Отсутствует плательщик";
    }
}
