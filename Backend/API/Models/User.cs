namespace HyteraAPI.Models;

public class User
{
    public int Id { get; set; }
    public string? Email { get; set; }
    public string? PasswordHash { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? AccessToken { get; set; }
    public string? UserRole { get; set; }
    public string? UserRoleName { get; set; }
    public string? UserAccessObjects { get; set; }
    public string? BPCode { get; set; }
    public string? BPName { get; set; }
    public string? BPRoleName { get; set; }
    public string? BPAccessObjects { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? LastLoginAt { get; set; }
    public bool IsActive { get; set; } = true;
}
