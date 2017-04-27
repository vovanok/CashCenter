using System;

namespace CashCenter.Common.Exceptions
{
    public class CustomerNotAppliedException : ApplicationException
    {
        public override string Message => "Отсутствует плательщик";
    }
}
