namespace HyteraAPI.Models.Requests;

public class LoginRequest
{
    public string? AppId { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}

public class UpdatePasswordRequest
{
    public string? UserId { get; set; }
    public string? Token { get; set; }
    public string? Password { get; set; }
}
