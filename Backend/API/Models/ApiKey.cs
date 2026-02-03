using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace HyteraAPI.Models;

/// <summary>
/// Represents an API key for partner authentication
/// </summary>
[Table("ApiKeys")]
public class ApiKey
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string PartnerKey { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string PartnerName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string KeyPrefix { get; set; } = string.Empty;

    /// <summary>
    /// JSON array of allowed scopes (e.g., ["inventory:read", "assets:write"])
    /// </summary>
    public string? Scopes { get; set; }

    /// <summary>
    /// JSON array of allowed IP addresses or CIDR ranges
    /// </summary>
    public string? AllowedIPs { get; set; }

    /// <summary>
    /// JSON array of allowed origins for CORS
    /// </summary>
    public string? AllowedOrigins { get; set; }

    public int RateLimitPerMinute { get; set; } = 60;

    public bool IsActive { get; set; } = true;

    public DateTime? ExpiresAt { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? LastUsedAt { get; set; }

    public long UsageCount { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    // Helper methods for JSON fields
    [NotMapped]
    public List<string> ScopesList
    {
        get
        {
            if (string.IsNullOrEmpty(Scopes)) return new List<string>();
            try { return JsonSerializer.Deserialize<List<string>>(Scopes) ?? new List<string>(); }
            catch { return new List<string>(); }
        }
    }

    [NotMapped]
    public List<string> AllowedIPsList
    {
        get
        {
            if (string.IsNullOrEmpty(AllowedIPs)) return new List<string>();
            try { return JsonSerializer.Deserialize<List<string>>(AllowedIPs) ?? new List<string>(); }
            catch { return new List<string>(); }
        }
    }

    [NotMapped]
    public List<string> AllowedOriginsList
    {
        get
        {
            if (string.IsNullOrEmpty(AllowedOrigins)) return new List<string>();
            try { return JsonSerializer.Deserialize<List<string>>(AllowedOrigins) ?? new List<string>(); }
            catch { return new List<string>(); }
        }
    }

    /// <summary>
    /// Check if this API key has a specific scope
    /// </summary>
    public bool HasScope(string scope)
    {
        var scopes = ScopesList;
        // "admin" scope grants all permissions
        if (scopes.Contains("admin")) return true;
        return scopes.Contains(scope);
    }

    /// <summary>
    /// Check if this API key is valid (active, not expired)
    /// </summary>
    public bool IsValid()
    {
        if (!IsActive) return false;
        if (ExpiresAt.HasValue && ExpiresAt.Value < DateTime.UtcNow) return false;
        return true;
    }
}

/// <summary>
/// Available API scopes for Hytera Data Core
/// </summary>
public static class HyteraScopes
{
    public const string InventoryRead = "inventory:read";
    public const string InventoryWrite = "inventory:write";
    public const string AssetsRead = "assets:read";
    public const string AssetsWrite = "assets:write";
    public const string AppsRead = "apps:read";
    public const string AppsWrite = "apps:write";
    public const string Admin = "admin";

    /// <summary>
    /// All available scopes
    /// </summary>
    public static readonly string[] AllScopes = new[]
    {
        InventoryRead, InventoryWrite,
        AssetsRead, AssetsWrite,
        AppsRead, AppsWrite,
        Admin
    };
}
