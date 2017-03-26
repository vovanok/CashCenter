using CashCenter.DbfRegistry;
using System.Linq;
using System;
using CashCenter.Common;
using CashCenter.CsvRegistry;
using CashCenter.Dal;

namespace CashCenter.IvEnergySales.BusinessLogic
{
    public class OfflineOrganizationSalesContext : BaseOrganizationSalesContext
    {
        private DbfRegistryController dbfRegistry;
        private CsvRegistryController csvRegistry = new CsvRegistryController();

        public OfflineOrganizationSalesContext(string contractNumberPart, string namePart, string innPart, string inputDbfFilename)
            : base(contractNumberPart, namePart, innPart)
        {
            throw new NotImplementedException();

            //dbfRegistry = new DbfRegistryController(inputDbfFilename);
            //var dbfOrganizations = dbfRegistry.GetOrganizations(/*contractNumberPart, namePart, innPart*/); //TODO

            //if (dbfOrganizations != null)
            //{
            //    Organizations = dbfOrganizations.Select(dbfOrganization =>
            //        new Organization(dbfOrganization.Id, dbfOrganization.DepartamentCode, dbfOrganization.ContractNumber,
            //            dbfOrganization.Name, dbfOrganization.Inn, dbfOrganization.Kpp)).ToList();
            //}
        }

        public override bool Pay(OrganizationPayment payment)
        {
            throw new NotImplementedException();

            //try
            //{
            //    csvRegistry.AddPayment(new CsvRegistry.Entities.OrganizationPayment(payment.Organization.Id, payment.DocumentNumber,
            //        payment.DepartmentCode, payment.CreateDate, payment.Comment, payment.Cost, payment.PaymentTypeId, payment.PaymentTypeName,
            //        payment.Code1C, payment.IncastCodePayment, payment.ReasonId));

            //    return true;
            //}
            //catch(Exception e)
            //{
            //    Log.ErrorWithException($"{ERROR_PREFIX}", e);
            //    return false;
            //}
        }
    }
}
