﻿namespace CashCenter.DbfRegistry.Entities
{
    public class DbfCustomer
    {
        public int Number { get; private set; }
        public string DepartmentCode { get; private set; }
        public int DayValue { get; private set; }
        public int NightValue { get; private set; }
        public decimal Balance { get; private set; }

        public DbfCustomer(int number, string departmentCode,
            int dayValue, int nightValue, decimal balance)
        {
            Number = number;
            DepartmentCode = departmentCode;
            DayValue = dayValue;
            NightValue = nightValue;
            Balance = balance;
        }
    }
}
