CREATE TABLE [dbo].[Region] (
    [Id]   INT            NOT NULL,
    [Name] NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Department] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Code]     NVARCHAR (50)  NOT NULL,
    [Name]     NVARCHAR (MAX) NOT NULL,
    [RegionId] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Department_ToRegion] FOREIGN KEY ([RegionId]) REFERENCES [dbo].[Region] ([Id])
);

CREATE TABLE [dbo].[PaymentReason] (
    [Id]   INT            NOT NULL,
    [Name] NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[EnergyCustomer] (
    [Id]           INT             IDENTITY (1, 1) NOT NULL,
    [DepartmentId] INT             NOT NULL,
    [Name]         NVARCHAR (MAX)  NOT NULL,
    [Address]      NVARCHAR (MAX)  NOT NULL,
    [DayValue]     INT             NOT NULL,
    [NightValue]   INT             NOT NULL,
    [Balance]      DECIMAL (18, 2) NOT NULL,
    [Penalty]      DECIMAL (18, 2) NOT NULL,
    [IsActive]     BIT             NOT NULL,
    [Email]        NVARCHAR (MAX)  NOT NULL,
    [Number]       INT             NOT NULL,
    [IsClosed] BIT NOT NULL, 
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EnergyCustomer_ToDepartment] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Department] ([Id])
);

CREATE TABLE [dbo].[EnergyCustomerPayment] (
    [Id]            INT             IDENTITY (1, 1) NOT NULL,
    [CustomerId]    INT             NOT NULL,
    [NewDayValue]   INT             NOT NULL,
    [NewNightValue] INT             NOT NULL,
    [Cost]          DECIMAL (18, 2) NOT NULL,
    [ReasonId]      INT             NOT NULL,
    [CreateDate]    DATETIME        NOT NULL,
    [Description]   NVARCHAR (MAX)  NOT NULL,
    [FiscalNumber]  INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_EnergyCustomerPayment_ToPaymentReason] FOREIGN KEY ([ReasonId]) REFERENCES [dbo].[PaymentReason] ([Id]),
    CONSTRAINT [FK_EnergyCustomerPayment_ToCustomer] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[EnergyCustomer] ([Id])
);

CREATE TABLE [dbo].[ArticlePriceType] (
    [Id]          INT            IDENTITY (1, 1) NOT NULL,
    [Name]        NVARCHAR (MAX) NOT NULL,
    [IsDefault]   BIT            NOT NULL,
    [IsWholesale] BIT            NOT NULL,
    [Code]        NVARCHAR (50)  NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Article] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Name]     NVARCHAR (MAX) NOT NULL,
    [Quantity] FLOAT (53)     NOT NULL,
    [Measure]  NVARCHAR (50)  NOT NULL,
    [Barcode]  NVARCHAR (50)  NOT NULL,
    [Code]     NVARCHAR (50)  NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[ArticlePrice] (
    [Id]                 INT          IDENTITY (1, 1) NOT NULL,
    [ArticleId]          INT          NOT NULL,
    [ArticlePriceTypeId] INT          NOT NULL,
    [EntryDate]          DATETIME     NOT NULL,
    [Value]              DECIMAL (18, 2) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ArticlePrice_To_ArticlePriceType] FOREIGN KEY ([ArticlePriceTypeId]) REFERENCES [dbo].[ArticlePriceType] ([Id]),
    CONSTRAINT [FK_ArticlePrice_To_Article] FOREIGN KEY ([ArticleId]) REFERENCES [dbo].[Article] ([Id])
);

CREATE TABLE [dbo].[ArticleSale] (
    [Id]             INT        IDENTITY (1, 1) NOT NULL,
    [ArticlePriceId] INT        NOT NULL,
    [Quantity]       FLOAT (53) NOT NULL,
    [Date]           DATETIME   NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ArticleSale_To_ArticlePrice] FOREIGN KEY ([ArticlePriceId]) REFERENCES [dbo].[ArticlePrice] ([Id])
);

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

CREATE TABLE [dbo].[GarbageCollectionPayment] (
    [Id]                  INT             IDENTITY (1, 1) NOT NULL,
    [FinancialPeriodCode] INT             NOT NULL,
    [CreateDate]          DATETIME        NOT NULL,
    [OrganizationCode]    INT             NOT NULL,
    [FilialCode]          INT             NOT NULL,
    [CustomerNumber]      INT             NOT NULL,
    [Cost]                DECIMAL (18, 2) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);