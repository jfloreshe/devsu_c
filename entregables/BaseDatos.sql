IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId] nvarchar(150) NOT NULL,
    [ProductVersion] nvarchar(32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [People] (
    [PersonId] bigint NOT NULL IDENTITY,
    [PersonalIdentifier] varchar(50) NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    [Gender] nvarchar(20) NOT NULL,
    [Age] int NOT NULL,
    [Address] nvarchar(200) NOT NULL,
    [Phone] varchar(50) NOT NULL,
    CONSTRAINT [PK_People] PRIMARY KEY ([PersonId])
    );
GO

CREATE TABLE [Customers] (
    [PersonId] bigint NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    [Password] varchar(50) NOT NULL,
    [State] bit NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([PersonId]),
    CONSTRAINT [FK_Customers_People_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [People] ([PersonId]) ON DELETE CASCADE
    );
GO

CREATE UNIQUE INDEX [IX_Customers_CustomerId] ON [Customers] ([CustomerId]) WHERE [CustomerId] IS NOT NULL;
GO

CREATE UNIQUE INDEX [IX_People_PersonalIdentifier] ON [People] ([PersonalIdentifier]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241111231305_CreateSchema', N'8.0.10');
GO

COMMIT;
GO

-----------------------------------------------MS Account
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId] nvarchar(150) NOT NULL,
    [ProductVersion] nvarchar(32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [Accounts] (
    [AccountId] bigint NOT NULL IDENTITY,
    [AccountNumber] nvarchar(450) NOT NULL,
    [AccountType] nvarchar(max) NOT NULL,
    [OpeningBalance] decimal(18,2) NOT NULL,
    [State] bit NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_Accounts] PRIMARY KEY ([AccountId])
    );
GO

CREATE TABLE [Customers] (
    [CustomerId] uniqueidentifier NOT NULL,
    [Name] nvarchar(200) NOT NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY ([CustomerId])
    );
GO

CREATE TABLE [Transactions] (
    [TransactionId] uniqueidentifier NOT NULL,
    [DateCreation] datetime2 NOT NULL,
    [Type] nvarchar(max) NOT NULL,
    [TransactionValue] decimal(18,2) NOT NULL,
    [Balance] decimal(18,2) NOT NULL,
    [AccountId] bigint NOT NULL,
    CONSTRAINT [PK_Transactions] PRIMARY KEY ([TransactionId]),
    CONSTRAINT [FK_Transactions_Accounts_AccountId] FOREIGN KEY ([AccountId]) REFERENCES [Accounts] ([AccountId]) ON DELETE CASCADE
    );
GO

CREATE TABLE [AccountTransactionLog] (
    [Id] bigint NOT NULL IDENTITY,
    [DateCreation] datetime2 NOT NULL,
    [TransactionId] uniqueidentifier NOT NULL,
    [PreviousTypeTransaction] nvarchar(max) NOT NULL,
    [NewTypeTransaction] nvarchar(max) NOT NULL,
    [PreviousTransactionValue] decimal(18,2) NOT NULL,
    [NewTransactionValue] decimal(18,2) NOT NULL,
    [PreviousBalance] decimal(18,2) NOT NULL,
    [NewBalance] decimal(18,2) NOT NULL,
    CONSTRAINT [PK_AccountTransactionLog] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AccountTransactionLog_Transactions_TransactionId] FOREIGN KEY ([TransactionId]) REFERENCES [Transactions] ([TransactionId]) ON DELETE CASCADE
    );
GO

CREATE INDEX [IX_AccountTransactionLog_TransactionId] ON [AccountTransactionLog] ([TransactionId]);
GO

CREATE UNIQUE INDEX [IX_Accounts_AccountNumber] ON [Accounts] ([AccountNumber]);
GO

CREATE INDEX [IX_Accounts_CustomerId] ON [Accounts] ([CustomerId]);
GO

CREATE INDEX [IX_Transactions_AccountId] ON [Transactions] ([AccountId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20241112094150_AddAccountMsSchema', N'8.0.10');
GO

COMMIT;
GO
