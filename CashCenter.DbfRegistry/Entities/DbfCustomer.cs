﻿namespace CashCenter.DbfRegistry.Entities
{
    public class DbfCustomer
    {
        public int Id { get; private set; }
        public string DepartmentCode { get; private set; }
        public int DayValue { get; private set; }
        public int NightValue { get; private set; }
        public decimal Balance { get; private set; }

        public DbfCustomer(int id, string departmentCode,
            int dayValue, int nightValue, decimal balance)
        {
            Id = id;
            DepartmentCode = departmentCode;
            DayValue = dayValue;
            NightValue = nightValue;
            Balance = balance;
        }
    }
}