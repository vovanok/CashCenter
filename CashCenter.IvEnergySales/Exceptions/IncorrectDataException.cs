using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CashCenter.IvEnergySales.Exceptions
{
    public class IncorrectDataException : UserException
    {
        public IEnumerable<string> DataNames { get; private set; }

        public IncorrectDataException(string dataName)
            : this(new[] { dataName })
        {
        }

        public IncorrectDataException(IEnumerable<string> errors)
        {
            DataNames = errors?.Where(item => !string.IsNullOrEmpty(item)) ?? new string[] { };
        }

        public override string Message
        {
            get
            {
                var sb = new StringBuilder();
                sb.AppendLine("Некорректные данные");
                if (DataNames != null)
                {
                    sb.AppendLine();
                    foreach(var dataName in DataNames)
                    {
                        sb.AppendLine($"- {dataName}");
                    }
                }

                return sb.ToString();
            }
        }
    }
}
