using System;

namespace CashCenter.Check
{
    public class CheckException : SystemException
    {
        public int Code { get; private set; }

        public string Description { get; private set; }

        public CheckException(int code, string description)
            : base()
        {
        }

        public override string Message => $"Код: {Code}. Описание: {Description}";
    }
}
