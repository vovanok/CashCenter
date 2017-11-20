ALTER TABLE [dbo].[WaterCustomerPayment]
 ADD CommissionValue DECIMAL (18, 2) DEFAULT (0) NOT NULL;

GO

UPDATE [dbo].[WaterCustomerPayment]
	SET CommissionValue = ROUND(Cost * ComissionPercent / 100, 2);

ALTER TABLE [dbo].[WaterCustomerPayment]
	DROP CONSTRAINT DF__WaterCust__Comis__19AACF41;

ALTER TABLE [dbo].[WaterCustomerPayment]
	DROP COLUMN ComissionPercent;

GO


ALTER TABLE [dbo].[GarbageCollectionPayment]
	ADD CommissionValue DECIMAL (18, 2) DEFAULT(0) NOT NULL;

GO

UPDATE [dbo].[GarbageCollectionPayment]
	SET CommissionValue = ROUND(Cost * CommissionPercent / 100, 2);

ALTER TABLE [dbo].[GarbageCollectionPayment]
	DROP COLUMN CommissionPercent;

GO


ALTER TABLE [dbo].[RepairPayment]
	ADD CommissionValue DECIMAL (18, 2) DEFAULT(0) NOT NULL;

GO

UPDATE [dbo].[RepairPayment]
	SET CommissionValue = ROUND(Cost * CommissionPercent / 100, 2);

ALTER TABLE [dbo].[RepairPayment]
	DROP COLUMN CommissionPercent;

GO