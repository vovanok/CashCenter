CREATE TABLE [dbo].[WaterCustomer] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [Number]         INT            NOT NULL,
    [Name]           NVARCHAR (MAX) NOT NULL,
    [Address]        NVARCHAR (MAX) NOT NULL,
    [CounterNumber1] INT            NOT NULL,
    [CounterNumber2] INT            NOT NULL,
    [CounterNumber3] INT            NOT NULL,
    [Email]          NVARCHAR (100) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[WaterCustomerPayment] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [CreateDate]   DATETIME        NOT NULL,
    [CustomerId]   INT             NOT NULL,
    [CounterCost1] DECIMAL (18, 2) NOT NULL,
    [CounterCost2] DECIMAL (18, 2) NOT NULL,
    [CounterCost3] DECIMAL (18, 2) NOT NULL,
    [Description]  NVARCHAR (MAX)  NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WaterCustomerPayment_ToWaterCustomer] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[WaterCustomer] ([Id])
);