using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CashCenter.Common.Exceptions
{
    public class IncorrectDataException : ApplicationException
    {
        public IEnumerable<string> DataNames { get; private set; }

        public IncorrectDataException(string dataName)
            : this(new[] { dataName })
        {
        }

        public IncorrectDataException(IEnumerable<string> dataNames)
        {
            DataNames = dataNames?.Where(item => !string.IsNullOrEmpty(item)) ?? new string[] { };
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
