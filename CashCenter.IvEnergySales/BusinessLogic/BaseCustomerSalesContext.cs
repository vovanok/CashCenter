using CashCenter.Common.DataEntities;
﻿using System.Collections.Generic;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public abstract class BaseCustomerSalesContext
    {
        protected const string ERROR_PREFIX = "Ошибка совершения платежа.";

        public Customer Customer { get; protected set; }
        public InfoForCheck InfoForCheck { get; protected set; }
        public List<PaymentReason> PaymentReasons { get; protected set; }

        public bool IsCustomerFinded
        {
            get { return Customer != null; }
        }

        public BaseCustomerSalesContext(int customerId)
        {
        }

        public abstract bool Pay(CustomerPayment payment);
    }
}
