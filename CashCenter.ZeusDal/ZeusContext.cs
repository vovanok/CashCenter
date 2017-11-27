using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CashCenter.ZeusDal
{
    public class ZeusContext
    {
        private string connectionString;

        public ZeusContext(string connectionString)
        {
            this.connectionString = connectionString;
        }


    }
}
