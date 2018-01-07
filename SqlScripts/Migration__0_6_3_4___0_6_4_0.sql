ALTER TABLE [dbo].[EnergyCustomer]
 ADD [PaymentDocumentIdentifier] NVARCHAR (20) DEFAULT ('') NOT NULL;

ALTER TABLE [dbo].[EnergyCustomer]
 ADD [HusIdentifier] NVARCHAR (20) DEFAULT ('') NOT NULL;

GO