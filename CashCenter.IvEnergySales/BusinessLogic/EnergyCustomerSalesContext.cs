﻿using CashCenter.Dal;
using CashCenter.Check;
using CashCenter.Common;
using CashCenter.IvEnergySales.Check;
using CashCenter.IvEnergySales.Common;
using System;
using System.Linq;
using System.Collections.Generic;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class EnergyCustomerSalesContext
    {
        protected const string PAY_ERROR_PREFIX = "Ошибка совершения платежа.";

        public Customer Customer { get; protected set; }
        public List<PaymentReason> PaymentReasons { get; protected set; }

        public bool IsCustomerFinded => Customer != null;

        public EnergyCustomerSalesContext(Department department, int customerNumber)
        {
            if (department == null)
                return;

            try
            {
                var customers = DalController.Instance.Customers.Where(customer => customer.Number == customerNumber && customer.IsActive);

                Customer = customers.FirstOrDefault(customer => customer.Department.Id == department.Id)
                    ?? customers.FirstOrDefault(customer => customer.Department.Code == department.Code);

                if (Customer == null)
                    return;

                PaymentReasons = DalController.Instance.PaymentReasons.ToList();
            }
            catch(Exception ex)
            {
                var message = "Ошибка поиска плательщика за электроэнергию";
                Log.Error(message, ex);
                Message.Error(message);
            }
        }

        private void ChangeEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
                return;

            if (!IsCustomerFinded)
                return;

            if (Customer.Email == email)
                return;

            Log.Info("Изменение email");

            if (!StringUtils.IsValidEmail(email))
            {
                var message = $"Адрес электронной почты имеет неверный формат ({email})";
                Log.Error(message);
                Message.Error(message);
                return;
            }

            Customer.Email = email;
            DalController.Instance.Save();

            Log.Info("Изменение email успешно завершено");
        }

        public void Pay(string email, int dayValue, int nightValue, decimal cost, int reasonId, string description, bool isWithoutCheck)
        {
            try
            {
                Log.Info($"Платеж за электроэнергию: email = {email}, dayValue = {dayValue}; nightValue = {nightValue}; cost = {cost}; reasonId = {reasonId}; description = {description}");

                if (!IsCustomerFinded)
                {
                    var message = $"{PAY_ERROR_PREFIX}\nОтсутствует плательщик.";
                    Log.Error(message);
                    Message.Error(message);
                    return;
                }

                var payment = new CustomerPayment
                {
                    CustomerId = Customer.Id,
                    NewDayValue = dayValue,
                    NewNightValue = nightValue,
                    Cost = cost,
                    ReasonId = reasonId,
                    CreateDate = DateTime.Now,
                    Description = description,
                    FiscalNumber = 0 // TODO: Fill fiscal
                };

                if (!payment.IsValid(Customer, out string validationErrorMessage))
                {
                    var message = $"{PAY_ERROR_PREFIX}\n{validationErrorMessage}";
                    Log.Error(message);
                    Message.Error(message);
                    return;
                }

                ChangeEmail(email);

                var paymentReasonName = PaymentReasons.FirstOrDefault(item => item.Id == payment.ReasonId)?.Name ?? string.Empty;

                if (isWithoutCheck || TryPrintChecks(payment.Cost, Customer.Department.Code, Customer.Number, Customer.Name, paymentReasonName, Customer.Email))
                {   
                    DalController.Instance.AddCustomerPayment(payment);
                }
            }
            catch(Exception ex)
            {
                var message = "Ошибка создание платежа за электроэнергию";
                Log.Error(message, ex);
                Message.Error(message);
            }
        }

        private bool TryPrintChecks(decimal cost, string departmentCode, int customerNumber,
            string customerName, string paymentReasonName, string customerEmail)
        {
            try
            {
                using (var waiter = new OperationWaiter())
                {
                    var check = new EnergyCustomerCheck(departmentCode, customerNumber, customerName,
                        paymentReasonName, Properties.Settings.Default.CasherName, cost, customerEmail);

                    CheckPrinter.Print(check);
                    return true;
                }
            }
            catch (Exception ex)
            {
                var errorMessage = "Ошибка печати чека";
                Log.Error(errorMessage, ex);
                Message.Error(errorMessage);
                return false;
            }
        }
    }
}
