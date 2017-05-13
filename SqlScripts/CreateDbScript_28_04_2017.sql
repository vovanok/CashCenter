CREATE TABLE [dbo].[WaterCustomer] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [Number]         INT            NOT NULL,
    [Name]           NVARCHAR (MAX) NOT NULL,
    [Address]        NVARCHAR (MAX) NOT NULL,
    [CounterNumber1] NVARCHAR (20)  NOT NULL,
    [CounterNumber2] NVARCHAR (20)  NOT NULL,
    [CounterNumber3] NVARCHAR (20)  NOT NULL,
    [CounterNumber4] NVARCHAR (20)  NOT NULL,
    [Email]          NVARCHAR (100) NOT NULL,
    [IsActive]       BIT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[WaterCustomerPayment] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [CreateDate]    DATETIME        NOT NULL,
    [CustomerId]    INT             NOT NULL,
    [CounterValue1] FLOAT           NOT NULL,
    [CounterValue2] FLOAT           NOT NULL,
    [CounterValue3] FLOAT           NOT NULL,
    [CounterValue4] FLOAT           NOT NULL,
    [Description]   NVARCHAR (MAX)  NOT NULL,
    [Penalty]       DECIMAL (18, 2) NOT NULL,
    [Cost]          DECIMAL (18, 2) NOT NULL,
    [FiscalNumber]  INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_WaterCustomerPayment_ToWaterCustomer] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[WaterCustomer] ([Id])
);