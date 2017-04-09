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
    [Url]      NVARCHAR (MAX) NOT NULL,
    [Path]     NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Department_ToRegion] FOREIGN KEY ([RegionId]) REFERENCES [dbo].[Region] ([Id])
);

CREATE TABLE [dbo].[PaymentReason] (
    [Id]   INT            NOT NULL,
    [Name] NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[PaymentKind] (
    [Id]         INT            NOT NULL,
    [Name]       NVARCHAR (MAX) NOT NULL,
    [TypeZeusId] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Organization] (
    [Id]             INT            IDENTITY (1, 1) NOT NULL,
    [ContractNumber] NVARCHAR (50)  NOT NULL,
    [Name]           NVARCHAR (MAX) NOT NULL,
    [Inn]            NVARCHAR (50)  NOT NULL,
    [Kpp]            NVARCHAR (50)  NOT NULL,
    [DepartmentId]   INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Organization_ToDepartment] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Department] ([Id])
);

CREATE TABLE [dbo].[OrganizationPayment] (
    [Id]              INT            NOT NULL,
    [OrganizationId]  INT            NOT NULL,
    [Date]            DATETIME       NOT NULL,
    [DocumentNumber]  INT            NOT NULL,
    [Comment]         NVARCHAR (MAX) NOT NULL,
    [Cost]            DECIMAL (18)   NOT NULL,
    [PaymentKindId]   INT            NOT NULL,
    [Code1C]          NVARCHAR (50)  NOT NULL,
    [IncastCode]      NVARCHAR (50)  NOT NULL,
    [PaymentReasonId] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_OrganizationPayment_ToPaymentReason] FOREIGN KEY ([PaymentReasonId]) REFERENCES [dbo].[PaymentReason] ([Id]),
    CONSTRAINT [FK_OrganizationPayment_ToPaymentKind] FOREIGN KEY ([PaymentKindId]) REFERENCES [dbo].[PaymentKind] ([Id]),
    CONSTRAINT [FK_OrganizationPayment_ToOrganization] FOREIGN KEY ([OrganizationId]) REFERENCES [dbo].[Organization] ([Id])
);

CREATE TABLE [dbo].[Customer] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [DepartmentId] INT            NOT NULL,
    [Name]         NVARCHAR (MAX) NOT NULL,
    [Address]      NVARCHAR (MAX) NOT NULL,
    [DayValue]     INT            NOT NULL,
    [NightValue]   INT            NOT NULL,
    [Balance]      DECIMAL (18)   NOT NULL,
    [Penalty]      DECIMAL (18)   NOT NULL,
    [IsActive]     BIT            NOT NULL,
    [Email]        NVARCHAR (MAX) NOT NULL,
    [Number]       INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_Customer_ToDepartment] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Department] ([Id])
);

CREATE TABLE [dbo].[CustomerPayment] (
    [Id]            INT            IDENTITY (1, 1) NOT NULL,
    [CustomerId]    INT            NOT NULL,
    [NewDayValue]   INT            NOT NULL,
    [NewNightValue] INT            NOT NULL,
    [Cost]          DECIMAL (18)   NOT NULL,
    [ReasonId]      INT            NOT NULL,
    [CreateDate]    DATETIME       NOT NULL,
    [Description]   NVARCHAR (MAX) NOT NULL,
    [PaymentKindId] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_CustomerPayment_ToPaymentReason] FOREIGN KEY ([ReasonId]) REFERENCES [dbo].[PaymentReason] ([Id]),
    CONSTRAINT [FK_CustomerPayment_ToPaymentKind] FOREIGN KEY ([PaymentKindId]) REFERENCES [dbo].[PaymentKind] ([Id]),
    CONSTRAINT [FK_CustomerPayment_ToCustomer] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[Customer] ([Id])
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
    [Value]              DECIMAL (18) NOT NULL,
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
