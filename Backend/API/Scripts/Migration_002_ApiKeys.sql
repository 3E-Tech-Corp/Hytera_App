-- ============================================================================
-- Migration 002: API Keys table and stored procedures
-- Hytera Data Core - API Key Authentication
-- ============================================================================

-- Create ApiKeys table
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ApiKeys')
BEGIN
    CREATE TABLE [dbo].[ApiKeys] (
        [Id]                 INT            IDENTITY(1,1) NOT NULL,
        [PartnerKey]         NVARCHAR(50)   NOT NULL,
        [PartnerName]        NVARCHAR(100)  NOT NULL,
        [ApiKey]             NVARCHAR(100)  NOT NULL,
        [KeyPrefix]          NVARCHAR(20)   NOT NULL,
        [Scopes]             NVARCHAR(MAX)  NULL,          -- JSON array
        [AllowedIPs]         NVARCHAR(MAX)  NULL,          -- JSON array
        [AllowedOrigins]     NVARCHAR(MAX)  NULL,          -- JSON array
        [RateLimitPerMinute] INT            NOT NULL DEFAULT 60,
        [IsActive]           BIT            NOT NULL DEFAULT 1,
        [ExpiresAt]          DATETIME2      NULL,
        [CreatedAt]          DATETIME2      NOT NULL DEFAULT GETUTCDATE(),
        [LastUsedAt]         DATETIME2      NULL,
        [UsageCount]         BIGINT         NOT NULL DEFAULT 0,
        [Description]        NVARCHAR(500)  NULL,
        CONSTRAINT [PK_ApiKeys] PRIMARY KEY CLUSTERED ([Id] ASC)
    );

    CREATE UNIQUE NONCLUSTERED INDEX [IX_ApiKeys_ApiKey] ON [dbo].[ApiKeys] ([ApiKey]);
    CREATE NONCLUSTERED INDEX [IX_ApiKeys_PartnerKey] ON [dbo].[ApiKeys] ([PartnerKey]);
    CREATE NONCLUSTERED INDEX [IX_ApiKeys_IsActive] ON [dbo].[ApiKeys] ([IsActive]);

    PRINT 'Created table: ApiKeys';
END
GO

-- ============================================================================
-- psp_ApiKey_GetByKey: Lookup API key by its value
-- ============================================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'psp_ApiKey_GetByKey')
    DROP PROCEDURE [dbo].[psp_ApiKey_GetByKey];
GO

CREATE PROCEDURE [dbo].[psp_ApiKey_GetByKey]
    @ApiKey NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [PartnerKey],
        [PartnerName],
        [ApiKey],
        [KeyPrefix],
        [Scopes],
        [AllowedIPs],
        [AllowedOrigins],
        [RateLimitPerMinute],
        [IsActive],
        [ExpiresAt],
        [CreatedAt],
        [LastUsedAt],
        [UsageCount],
        [Description]
    FROM [dbo].[ApiKeys]
    WHERE [ApiKey] = @ApiKey;
END
GO

-- ============================================================================
-- psp_ApiKey_GetAll: List all API keys
-- ============================================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'psp_ApiKey_GetAll')
    DROP PROCEDURE [dbo].[psp_ApiKey_GetAll];
GO

CREATE PROCEDURE [dbo].[psp_ApiKey_GetAll]
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        [Id],
        [PartnerKey],
        [PartnerName],
        [ApiKey],
        [KeyPrefix],
        [Scopes],
        [AllowedIPs],
        [AllowedOrigins],
        [RateLimitPerMinute],
        [IsActive],
        [ExpiresAt],
        [CreatedAt],
        [LastUsedAt],
        [UsageCount],
        [Description]
    FROM [dbo].[ApiKeys]
    ORDER BY [CreatedAt] DESC;
END
GO

-- ============================================================================
-- psp_ApiKey_Create: Insert a new API key
-- ============================================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'psp_ApiKey_Create')
    DROP PROCEDURE [dbo].[psp_ApiKey_Create];
GO

CREATE PROCEDURE [dbo].[psp_ApiKey_Create]
    @PartnerKey         NVARCHAR(50),
    @PartnerName        NVARCHAR(100),
    @ApiKey             NVARCHAR(100),
    @KeyPrefix          NVARCHAR(20),
    @Scopes             NVARCHAR(MAX) = NULL,
    @AllowedIPs         NVARCHAR(MAX) = NULL,
    @AllowedOrigins     NVARCHAR(MAX) = NULL,
    @RateLimitPerMinute INT = 60,
    @ExpiresAt          DATETIME2 = NULL,
    @Description        NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO [dbo].[ApiKeys]
        ([PartnerKey], [PartnerName], [ApiKey], [KeyPrefix], [Scopes],
         [AllowedIPs], [AllowedOrigins], [RateLimitPerMinute], [ExpiresAt], [Description])
    VALUES
        (@PartnerKey, @PartnerName, @ApiKey, @KeyPrefix, @Scopes,
         @AllowedIPs, @AllowedOrigins, @RateLimitPerMinute, @ExpiresAt, @Description);

    -- Return the newly created row
    SELECT
        [Id],
        [PartnerKey],
        [PartnerName],
        [ApiKey],
        [KeyPrefix],
        [Scopes],
        [AllowedIPs],
        [AllowedOrigins],
        [RateLimitPerMinute],
        [IsActive],
        [ExpiresAt],
        [CreatedAt],
        [LastUsedAt],
        [UsageCount],
        [Description]
    FROM [dbo].[ApiKeys]
    WHERE [Id] = SCOPE_IDENTITY();
END
GO

-- ============================================================================
-- psp_ApiKey_Update: Update API key properties
-- ============================================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'psp_ApiKey_Update')
    DROP PROCEDURE [dbo].[psp_ApiKey_Update];
GO

CREATE PROCEDURE [dbo].[psp_ApiKey_Update]
    @Id                 INT,
    @PartnerName        NVARCHAR(100) = NULL,
    @Scopes             NVARCHAR(MAX) = NULL,
    @AllowedIPs         NVARCHAR(MAX) = NULL,
    @AllowedOrigins     NVARCHAR(MAX) = NULL,
    @RateLimitPerMinute INT = NULL,
    @IsActive           BIT = NULL,
    @ExpiresAt          DATETIME2 = NULL,
    @Description        NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[ApiKeys]
    SET
        [PartnerName]        = COALESCE(@PartnerName, [PartnerName]),
        [Scopes]             = COALESCE(@Scopes, [Scopes]),
        [AllowedIPs]         = COALESCE(@AllowedIPs, [AllowedIPs]),
        [AllowedOrigins]     = COALESCE(@AllowedOrigins, [AllowedOrigins]),
        [RateLimitPerMinute] = COALESCE(@RateLimitPerMinute, [RateLimitPerMinute]),
        [IsActive]           = COALESCE(@IsActive, [IsActive]),
        [ExpiresAt]          = COALESCE(@ExpiresAt, [ExpiresAt]),
        [Description]        = COALESCE(@Description, [Description])
    WHERE [Id] = @Id;

    -- Return the updated row
    SELECT
        [Id],
        [PartnerKey],
        [PartnerName],
        [ApiKey],
        [KeyPrefix],
        [Scopes],
        [AllowedIPs],
        [AllowedOrigins],
        [RateLimitPerMinute],
        [IsActive],
        [ExpiresAt],
        [CreatedAt],
        [LastUsedAt],
        [UsageCount],
        [Description]
    FROM [dbo].[ApiKeys]
    WHERE [Id] = @Id;
END
GO

-- ============================================================================
-- psp_ApiKey_Delete: Soft delete (set IsActive = 0)
-- ============================================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'psp_ApiKey_Delete')
    DROP PROCEDURE [dbo].[psp_ApiKey_Delete];
GO

CREATE PROCEDURE [dbo].[psp_ApiKey_Delete]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[ApiKeys]
    SET [IsActive] = 0
    WHERE [Id] = @Id;

    SELECT @@ROWCOUNT AS [RowsAffected];
END
GO

-- ============================================================================
-- psp_ApiKey_Regenerate: Replace the key value for an existing record
-- ============================================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'psp_ApiKey_Regenerate')
    DROP PROCEDURE [dbo].[psp_ApiKey_Regenerate];
GO

CREATE PROCEDURE [dbo].[psp_ApiKey_Regenerate]
    @Id          INT,
    @NewApiKey   NVARCHAR(100),
    @NewKeyPrefix NVARCHAR(20)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[ApiKeys]
    SET
        [ApiKey]    = @NewApiKey,
        [KeyPrefix] = @NewKeyPrefix
    WHERE [Id] = @Id;

    -- Return the updated row
    SELECT
        [Id],
        [PartnerKey],
        [PartnerName],
        [ApiKey],
        [KeyPrefix],
        [Scopes],
        [AllowedIPs],
        [AllowedOrigins],
        [RateLimitPerMinute],
        [IsActive],
        [ExpiresAt],
        [CreatedAt],
        [LastUsedAt],
        [UsageCount],
        [Description]
    FROM [dbo].[ApiKeys]
    WHERE [Id] = @Id;
END
GO

-- ============================================================================
-- psp_ApiKey_RecordUsage: Increment usage count and set last used timestamp
-- ============================================================================
IF EXISTS (SELECT * FROM sys.procedures WHERE name = 'psp_ApiKey_RecordUsage')
    DROP PROCEDURE [dbo].[psp_ApiKey_RecordUsage];
GO

CREATE PROCEDURE [dbo].[psp_ApiKey_RecordUsage]
    @ApiKey NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE [dbo].[ApiKeys]
    SET
        [UsageCount] = [UsageCount] + 1,
        [LastUsedAt] = GETUTCDATE()
    WHERE [ApiKey] = @ApiKey
      AND [IsActive] = 1;
END
GO

PRINT 'Migration 002: ApiKeys table and stored procedures created successfully.';
GO
