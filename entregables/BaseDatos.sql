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

