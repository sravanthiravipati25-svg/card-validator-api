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
CREATE TABLE [CardValidationRecords] (
    [Id] int NOT NULL IDENTITY,
    [CardNumberMasked] nvarchar(25) NOT NULL,
    [CardNumberHash] nvarchar(128) NOT NULL,
    [IsValid] bit NOT NULL,
    [IssuerNetwork] nvarchar(50) NULL,
    [FailureReason] nvarchar(200) NULL,
    [ValidatedAtUtc] datetime2 NOT NULL,
    [Source] nvarchar(10) NOT NULL,
    [BatchId] nvarchar(50) NULL,
    CONSTRAINT [PK_CardValidationRecords] PRIMARY KEY ([Id])
);

CREATE INDEX [IX_CardValidationRecords_BatchId] ON [CardValidationRecords] ([BatchId]);

CREATE INDEX [IX_CardValidationRecords_ValidatedAtUtc] ON [CardValidationRecords] ([ValidatedAtUtc]);

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260724105911_InitialCreate', N'10.0.10');

COMMIT;
GO

BEGIN TRANSACTION;
INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260724160829_InitialCreateForDeploy', N'10.0.10');

COMMIT;
GO

