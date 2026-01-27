-- Migration_001_InitialSchema.sql
-- Initial database schema for Hytera App
-- Created: 2024

PRINT 'Starting Migration_001_InitialSchema...'

-- Users table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE [dbo].[Users] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [Email] NVARCHAR(255) NULL,
        [PasswordHash] NVARCHAR(500) NULL,
        [FirstName] NVARCHAR(100) NULL,
        [LastName] NVARCHAR(100) NULL,
        [AccessToken] NVARCHAR(500) NULL,
        [UserRole] NVARCHAR(50) NULL,
        [UserRoleName] NVARCHAR(100) NULL,
        [UserAccessObjects] NVARCHAR(MAX) NULL,
        [BPCode] NVARCHAR(50) NULL,
        [BPName] NVARCHAR(255) NULL,
        [BPRoleName] NVARCHAR(100) NULL,
        [BPAccessObjects] NVARCHAR(MAX) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [LastLoginAt] DATETIME2 NULL,
        [IsActive] BIT NOT NULL DEFAULT 1
    )
    PRINT 'Created Users table'
END
ELSE
    PRINT 'Users table already exists'
GO

-- Items table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Items')
BEGIN
    CREATE TABLE [dbo].[Items] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [ItemCode] NVARCHAR(50) NULL,
        [ItemName] NVARCHAR(255) NULL,
        [ItemType] NVARCHAR(50) NULL,
        [Instock] INT NOT NULL DEFAULT 0,
        [OnOrder] INT NOT NULL DEFAULT 0,
        [MSRP] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [DealerPrice] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [ItemImageUrl] NVARCHAR(500) NULL,
        [ItemLinkUrl] NVARCHAR(500) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [IsActive] BIT NOT NULL DEFAULT 1
    )
    PRINT 'Created Items table'
END
ELSE
    PRINT 'Items table already exists'
GO

-- GameScores table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GameScores')
BEGIN
    CREATE TABLE [dbo].[GameScores] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [AppId] NVARCHAR(50) NULL,
        [EventId] INT NOT NULL DEFAULT 0,
        [MatchId] INT NOT NULL DEFAULT 0,
        [GameNumber] INT NOT NULL DEFAULT 0,
        [FunTimeId] NVARCHAR(50) NULL,
        [FunTimeId1] NVARCHAR(50) NULL,
        [FunTimeId2] NVARCHAR(50) NULL,
        [FunTimeId3] NVARCHAR(50) NULL,
        [FunTimeId4] NVARCHAR(50) NULL,
        [Score12] NVARCHAR(20) NULL,
        [Score34] NVARCHAR(20) NULL,
        [StartTime] DATETIME2 NULL,
        [EndTime] DATETIME2 NULL,
        [GPSLat] NVARCHAR(50) NULL,
        [GPSLng] NVARCHAR(50) NULL,
        [GameSequence] NVARCHAR(50) NULL,
        [RemoteIp] NVARCHAR(50) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    )
    PRINT 'Created GameScores table'
END
ELSE
    PRINT 'GameScores table already exists'
GO

-- Assets table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Assets')
BEGIN
    CREATE TABLE [dbo].[Assets] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [ShowName] NVARCHAR(255) NULL,
        [PhysicalPath] NVARCHAR(500) NULL,
        [PhysicalName] NVARCHAR(255) NULL,
        [ContentType] NVARCHAR(100) NULL,
        [FileSize] BIGINT NOT NULL DEFAULT 0,
        [UserId] INT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [UpdatedAt] DATETIME2 NULL,
        [IsActive] BIT NOT NULL DEFAULT 1
    )
    PRINT 'Created Assets table'
END
ELSE
    PRINT 'Assets table already exists'
GO

-- Languages table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Languages')
BEGIN
    CREATE TABLE [dbo].[Languages] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [LanguageCode] NVARCHAR(20) NULL,
        [LanguageName] NVARCHAR(100) NULL,
        [Version] INT NOT NULL DEFAULT 1,
        [IndexFileUrl] NVARCHAR(500) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1
    )
    PRINT 'Created Languages table'
END
ELSE
    PRINT 'Languages table already exists'
GO

-- VoiceSets table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VoiceSets')
BEGIN
    CREATE TABLE [dbo].[VoiceSets] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [VoiceSetCode] NVARCHAR(50) NULL,
        [VoiceSetName] NVARCHAR(100) NULL,
        [Description] NVARCHAR(500) NULL,
        [Version] NVARCHAR(20) NULL,
        [AudioFileUrl] NVARCHAR(500) NULL,
        [IndexFileUrl] NVARCHAR(500) NULL,
        [LanguageCode] NVARCHAR(20) NULL,
        [LanguageIndexUrl] NVARCHAR(500) NULL,
        [IsActive] BIT NOT NULL DEFAULT 1
    )
    PRINT 'Created VoiceSets table'
END
ELSE
    PRINT 'VoiceSets table already exists'
GO

-- AppVersions table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AppVersions')
BEGIN
    CREATE TABLE [dbo].[AppVersions] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [AppId] NVARCHAR(50) NULL,
        [OS] NVARCHAR(50) NULL,
        [Version] NVARCHAR(20) NULL,
        [DownloadUrl] NVARCHAR(500) NULL,
        [ReleaseNotes] NVARCHAR(MAX) NULL,
        [IsRequired] BIT NOT NULL DEFAULT 0,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsActive] BIT NOT NULL DEFAULT 1
    )
    PRINT 'Created AppVersions table'
END
ELSE
    PRINT 'AppVersions table already exists'
GO

-- AppRocs table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AppRocs')
BEGIN
    CREATE TABLE [dbo].[AppRocs] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [AppId] NVARCHAR(50) NULL,
        [FunTimeId] NVARCHAR(50) NULL,
        [RocId] NVARCHAR(50) NULL,
        [Roc1] NVARCHAR(255) NULL,
        [Roc2] NVARCHAR(255) NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE()
    )
    PRINT 'Created AppRocs table'
END
ELSE
    PRINT 'AppRocs table already exists'
GO

-- GptPrompts table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'GptPrompts')
BEGIN
    CREATE TABLE [dbo].[GptPrompts] (
        [Id] INT IDENTITY(1,1) NOT NULL PRIMARY KEY,
        [QueryType] NVARCHAR(50) NULL,
        [SystemText] NVARCHAR(MAX) NULL,
        [UserText] NVARCHAR(MAX) NULL,
        [GptModel] NVARCHAR(50) NULL,
        [Temperature] DECIMAL(5,2) NOT NULL DEFAULT 0,
        [IsActive] BIT NOT NULL DEFAULT 1
    )
    PRINT 'Created GptPrompts table'
END
ELSE
    PRINT 'GptPrompts table already exists'
GO

-- Create indexes
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Users_Email')
BEGIN
    CREATE INDEX IX_Users_Email ON [dbo].[Users] ([Email])
    PRINT 'Created index IX_Users_Email'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Items_ItemCode')
BEGIN
    CREATE INDEX IX_Items_ItemCode ON [dbo].[Items] ([ItemCode])
    PRINT 'Created index IX_Items_ItemCode'
END
GO

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_GameScores_EventId_MatchId')
BEGIN
    CREATE INDEX IX_GameScores_EventId_MatchId ON [dbo].[GameScores] ([EventId], [MatchId])
    PRINT 'Created index IX_GameScores_EventId_MatchId'
END
GO

PRINT 'Migration_001_InitialSchema completed successfully'
GO
