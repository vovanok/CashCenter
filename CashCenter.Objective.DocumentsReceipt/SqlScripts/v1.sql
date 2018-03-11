CREATE TABLE [dbo].[Department] (
    [Id]   INT            IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (MAX) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[SettlementCenter] (
    [Id]           INT            IDENTITY (1, 1) NOT NULL,
    [Name]         NVARCHAR (MAX) NOT NULL,
    [DepartmentId] INT            NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_SettlementCenter_Department] FOREIGN KEY ([DepartmentId]) REFERENCES [dbo].[Department] ([Id])
);

CREATE TABLE [dbo].[ReceiptDocument] (
    [Id]                 INT             IDENTITY (1, 1) NOT NULL,
    [Name]               NVARCHAR (MAX)  NOT NULL,
    [PaymentsCountSum]   INT             NOT NULL,
    [PaymentsTotalSum]   DECIMAL (18, 2) NOT NULL,
    [SettlementCenterId] INT             NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_ReceiptDocument_SettlementCenter] FOREIGN KEY ([SettlementCenterId]) REFERENCES [dbo].[SettlementCenter] ([Id])
);