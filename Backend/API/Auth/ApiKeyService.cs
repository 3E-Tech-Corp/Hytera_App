using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Dapper;
using HyteraAPI.Models;

namespace HyteraAPI.Auth;

public interface IApiKeyService
{
    Task<ApiKey?> ValidateKeyAsync(string apiKey);
    Task<bool> HasScopeAsync(string apiKey, string scope);
    Task RecordUsageAsync(string apiKey);
    Task<List<ApiKeyResponse>> GetAllAsync();
    Task<ApiKeyCreatedResponse> CreateAsync(CreateApiKeyRequest request);
    Task<ApiKeyResponse?> UpdateAsync(int id, UpdateApiKeyRequest request);
    Task<bool> DeleteAsync(int id);
    Task<ApiKeyCreatedResponse?> RegenerateAsync(int id);
    void InvalidateCache(string apiKey);
    void ClearCache();
}

public class ApiKeyService : IApiKeyService
{
    private readonly string _connectionString;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ApiKeyService> _logger;
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);
    private const string CacheKeyPrefix = "apikey_";

    public ApiKeyService(
        IConfiguration configuration,
        IMemoryCache cache,
        ILogger<ApiKeyService> logger)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("DefaultConnection not configured");
        _cache = cache;
        _logger = logger;
    }

    public async Task<ApiKey?> ValidateKeyAsync(string apiKey)
    {
        if (string.IsNullOrEmpty(apiKey)) return null;

        var cacheKey = CacheKeyPrefix + apiKey;

        if (_cache.TryGetValue(cacheKey, out ApiKey? cachedKey))
        {
            return cachedKey?.IsValid() == true ? cachedKey : null;
        }

        using var conn = new SqlConnection(_connectionString);
        var row = await conn.QuerySingleOrDefaultAsync<ApiKeyDbRow>(
            "psp_ApiKey_GetByKey",
            new { ApiKey = apiKey },
            commandType: System.Data.CommandType.StoredProcedure);

        if (row == null)
        {
            _cache.Set(cacheKey, (ApiKey?)null, TimeSpan.FromSeconds(30));
            return null;
        }

        var apiKeyModel = MapToModel(row);
        _cache.Set(cacheKey, apiKeyModel, CacheDuration);

        return apiKeyModel.IsValid() ? apiKeyModel : null;
    }

    public async Task<bool> HasScopeAsync(string apiKey, string scope)
    {
        var key = await ValidateKeyAsync(apiKey);
        return key?.HasScope(scope) ?? false;
    }

    public async Task RecordUsageAsync(string apiKey)
    {
        try
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.ExecuteAsync(
                "psp_ApiKey_RecordUsage",
                new { ApiKey = apiKey },
                commandType: System.Data.CommandType.StoredProcedure);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to record API key usage");
        }
    }

    public async Task<List<ApiKeyResponse>> GetAllAsync()
    {
        using var conn = new SqlConnection(_connectionString);
        var rows = await conn.QueryAsync<ApiKeyDbRow>(
            "psp_ApiKey_GetAll",
            commandType: System.Data.CommandType.StoredProcedure);

        return rows.Select(MapToResponse).ToList();
    }

    public async Task<ApiKeyCreatedResponse> CreateAsync(CreateApiKeyRequest request)
    {
        var (apiKey, keyPrefix) = GenerateApiKey(request.PartnerKey);

        using var conn = new SqlConnection(_connectionString);
        var row = await conn.QuerySingleAsync<ApiKeyDbRow>(
            "psp_ApiKey_Create",
            new
            {
                request.PartnerKey,
                request.PartnerName,
                ApiKey = apiKey,
                KeyPrefix = keyPrefix,
                Scopes = request.Scopes?.Count > 0 ? JsonSerializer.Serialize(request.Scopes) : null,
                AllowedIPs = request.AllowedIPs?.Count > 0 ? JsonSerializer.Serialize(request.AllowedIPs) : null,
                AllowedOrigins = request.AllowedOrigins?.Count > 0 ? JsonSerializer.Serialize(request.AllowedOrigins) : null,
                request.RateLimitPerMinute,
                request.ExpiresAt,
                request.Description
            },
            commandType: System.Data.CommandType.StoredProcedure);

        _logger.LogInformation("API key created for partner {PartnerKey}", request.PartnerKey);

        return new ApiKeyCreatedResponse
        {
            Id = row.Id,
            PartnerKey = row.PartnerKey,
            PartnerName = row.PartnerName,
            ApiKey = apiKey,
            KeyMasked = keyPrefix + "...",
            KeyPrefix = row.KeyPrefix,
            Scopes = ParseJsonArray(row.Scopes),
            AllowedIPs = ParseJsonArrayNullable(row.AllowedIPs),
            AllowedOrigins = ParseJsonArrayNullable(row.AllowedOrigins),
            RateLimitPerMinute = row.RateLimitPerMinute,
            IsActive = row.IsActive,
            ExpiresAt = row.ExpiresAt,
            CreatedAt = row.CreatedAt,
            Description = row.Description
        };
    }

    public async Task<ApiKeyResponse?> UpdateAsync(int id, UpdateApiKeyRequest request)
    {
        using var conn = new SqlConnection(_connectionString);
        var row = await conn.QuerySingleOrDefaultAsync<ApiKeyDbRow>(
            "psp_ApiKey_Update",
            new
            {
                Id = id,
                request.PartnerName,
                Scopes = request.Scopes != null ? JsonSerializer.Serialize(request.Scopes) : null,
                AllowedIPs = request.AllowedIPs != null ? JsonSerializer.Serialize(request.AllowedIPs) : null,
                AllowedOrigins = request.AllowedOrigins != null ? JsonSerializer.Serialize(request.AllowedOrigins) : null,
                request.RateLimitPerMinute,
                request.IsActive,
                request.ExpiresAt,
                request.Description
            },
            commandType: System.Data.CommandType.StoredProcedure);

        if (row != null)
        {
            ClearCache();
            _logger.LogInformation("API key {Id} updated", id);
        }

        return row != null ? MapToResponse(row) : null;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        using var conn = new SqlConnection(_connectionString);
        var result = await conn.QuerySingleAsync<int>(
            "psp_ApiKey_Delete",
            new { Id = id },
            commandType: System.Data.CommandType.StoredProcedure);

        if (result > 0)
        {
            ClearCache();
            _logger.LogInformation("API key {Id} soft-deleted", id);
        }

        return result > 0;
    }

    public async Task<ApiKeyCreatedResponse?> RegenerateAsync(int id)
    {
        using var conn = new SqlConnection(_connectionString);

        // Get existing key to retrieve partner key
        var existing = await conn.QuerySingleOrDefaultAsync<dynamic>(
            "SELECT PartnerKey FROM ApiKeys WHERE Id = @Id",
            new { Id = id });

        if (existing == null) return null;

        var (newApiKey, keyPrefix) = GenerateApiKey((string)existing.PartnerKey);

        var row = await conn.QuerySingleAsync<ApiKeyDbRow>(
            "psp_ApiKey_Regenerate",
            new
            {
                Id = id,
                NewApiKey = newApiKey,
                NewKeyPrefix = keyPrefix
            },
            commandType: System.Data.CommandType.StoredProcedure);

        ClearCache();
        _logger.LogInformation("API key {Id} regenerated for partner {PartnerKey}", id, (string)existing.PartnerKey);

        return new ApiKeyCreatedResponse
        {
            Id = row.Id,
            PartnerKey = row.PartnerKey,
            PartnerName = row.PartnerName,
            ApiKey = newApiKey,
            KeyMasked = keyPrefix + "...",
            KeyPrefix = row.KeyPrefix,
            Scopes = ParseJsonArray(row.Scopes),
            AllowedIPs = ParseJsonArrayNullable(row.AllowedIPs),
            AllowedOrigins = ParseJsonArrayNullable(row.AllowedOrigins),
            RateLimitPerMinute = row.RateLimitPerMinute,
            IsActive = row.IsActive,
            ExpiresAt = row.ExpiresAt,
            CreatedAt = row.CreatedAt,
            Description = row.Description
        };
    }

    public void InvalidateCache(string apiKey)
    {
        _cache.Remove(CacheKeyPrefix + apiKey);
    }

    public void ClearCache()
    {
        _logger.LogDebug("API key cache clear requested (entries will expire naturally)");
    }

    /// <summary>
    /// Generate a new API key with hk_ prefix
    /// </summary>
    private static (string apiKey, string keyPrefix) GenerateApiKey(string partnerKey)
    {
        var shortPartner = partnerKey.Length > 4 ? partnerKey[..4].ToLower() : partnerKey.ToLower();
        var keyPrefix = $"hk_{shortPartner}_";

        var randomBytes = RandomNumberGenerator.GetBytes(32);
        var randomPart = Convert.ToBase64String(randomBytes)
            .Replace("+", "")
            .Replace("/", "")
            .Replace("=", "")
            [..32];

        var apiKey = keyPrefix + randomPart;
        return (apiKey, keyPrefix);
    }

    #region Mapping Helpers

    private static ApiKey MapToModel(ApiKeyDbRow row) => new()
    {
        Id = row.Id,
        PartnerKey = row.PartnerKey,
        PartnerName = row.PartnerName,
        Key = row.ApiKey,
        KeyPrefix = row.KeyPrefix,
        Scopes = row.Scopes,
        AllowedIPs = row.AllowedIPs,
        AllowedOrigins = row.AllowedOrigins,
        RateLimitPerMinute = row.RateLimitPerMinute,
        IsActive = row.IsActive,
        ExpiresAt = row.ExpiresAt,
        CreatedAt = row.CreatedAt,
        LastUsedAt = row.LastUsedAt,
        UsageCount = row.UsageCount,
        Description = row.Description
    };

    private static ApiKeyResponse MapToResponse(ApiKeyDbRow row) => new()
    {
        Id = row.Id,
        PartnerKey = row.PartnerKey,
        PartnerName = row.PartnerName,
        KeyMasked = row.KeyPrefix + "...",
        KeyPrefix = row.KeyPrefix,
        Scopes = ParseJsonArray(row.Scopes),
        AllowedIPs = ParseJsonArrayNullable(row.AllowedIPs),
        AllowedOrigins = ParseJsonArrayNullable(row.AllowedOrigins),
        RateLimitPerMinute = row.RateLimitPerMinute,
        IsActive = row.IsActive,
        ExpiresAt = row.ExpiresAt,
        CreatedAt = row.CreatedAt,
        LastUsedAt = row.LastUsedAt,
        UsageCount = row.UsageCount,
        Description = row.Description
    };

    private static List<string> ParseJsonArray(string? json)
    {
        if (string.IsNullOrEmpty(json)) return new List<string>();
        try { return JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>(); }
        catch { return new List<string>(); }
    }

    private static List<string>? ParseJsonArrayNullable(string? json)
    {
        if (string.IsNullOrEmpty(json)) return null;
        try { return JsonSerializer.Deserialize<List<string>>(json); }
        catch { return null; }
    }

    #endregion

    #region DB Row Classes

    private class ApiKeyDbRow
    {
        public int Id { get; set; }
        public string PartnerKey { get; set; } = string.Empty;
        public string PartnerName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public string KeyPrefix { get; set; } = string.Empty;
        public string? Scopes { get; set; }
        public string? AllowedIPs { get; set; }
        public string? AllowedOrigins { get; set; }
        public int RateLimitPerMinute { get; set; }
        public bool IsActive { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public long UsageCount { get; set; }
        public string? Description { get; set; }
    }

    #endregion
}

#region API Key DTOs

public class ApiKeyResponse
{
    public int Id { get; set; }
    public string PartnerKey { get; set; } = string.Empty;
    public string PartnerName { get; set; } = string.Empty;
    public string KeyMasked { get; set; } = string.Empty;
    public string KeyPrefix { get; set; } = string.Empty;
    public List<string> Scopes { get; set; } = new();
    public List<string>? AllowedIPs { get; set; }
    public List<string>? AllowedOrigins { get; set; }
    public int RateLimitPerMinute { get; set; }
    public bool IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public long UsageCount { get; set; }
    public string? Description { get; set; }
}

public class ApiKeyCreatedResponse : ApiKeyResponse
{
    public string ApiKey { get; set; } = string.Empty;
}

public class CreateApiKeyRequest
{
    public string PartnerKey { get; set; } = string.Empty;
    public string PartnerName { get; set; } = string.Empty;
    public List<string> Scopes { get; set; } = new();
    public List<string>? AllowedIPs { get; set; }
    public List<string>? AllowedOrigins { get; set; }
    public int RateLimitPerMinute { get; set; } = 60;
    public DateTime? ExpiresAt { get; set; }
    public string? Description { get; set; }
}

public class UpdateApiKeyRequest
{
    public string? PartnerName { get; set; }
    public List<string>? Scopes { get; set; }
    public List<string>? AllowedIPs { get; set; }
    public List<string>? AllowedOrigins { get; set; }
    public int? RateLimitPerMinute { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public string? Description { get; set; }
}

#endregion
