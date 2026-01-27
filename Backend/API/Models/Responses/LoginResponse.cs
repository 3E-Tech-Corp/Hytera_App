namespace HyteraAPI.Models.Responses;

public class LoginResponse
{
    public int Status { get; set; }
    public string? Message { get; set; }
    public string? AccessToken { get; set; }
    public string? UserId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserRole { get; set; }
    public string? UserRoleName { get; set; }
    public string? UserAccessObjects { get; set; }
    public string? BPCode { get; set; }
    public string? BPName { get; set; }
    public string? BPRoleName { get; set; }
    public string? BPAccessObjects { get; set; }
}

public class UpdatePasswordResponse
{
    public int Status { get; set; }
    public string? Message { get; set; }
    public string? UserId { get; set; }
    public string? Token { get; set; }
}
