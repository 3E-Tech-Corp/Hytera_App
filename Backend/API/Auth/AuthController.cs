using System.Security.Claims;
using HyteraAPI.Models;
using HyteraAPI.Models.Requests;
using HyteraAPI.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HyteraAPI.Auth;

[Route("auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IJwtService _jwtService;
    private readonly IDatabaseService _databaseService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        IJwtService jwtService,
        IDatabaseService databaseService,
        ILogger<AuthController> logger)
    {
        _jwtService = jwtService;
        _databaseService = databaseService;
        _logger = logger;
    }

    /// <summary>
    /// Login with email and password, returns JWT token
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthTokenResponse>>> Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.Password))
        {
            return BadRequest(ApiResponse<AuthTokenResponse>.Fail("Email and password are required."));
        }

        var remoteIp = _databaseService.GetClientIpAddress(HttpContext);

        var parameters = new Dictionary<string, object>
        {
            { "RemoteIP", remoteIp },
            { "APPID", request.AppId ?? "HyteraAPI" },
            { "Email", request.Email },
            { "PWD", request.Password }
        };

        var dataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_CheckSAP", parameters);
        var loginResult = ParseLoginDataSet(dataSet);

        if (loginResult == null || loginResult.Status < 0)
        {
            _logger.LogWarning("Login failed for {Email} from {IP}: {Message}",
                request.Email, remoteIp, loginResult?.Message ?? "No data");
            return Unauthorized(ApiResponse<AuthTokenResponse>.Fail(
                loginResult?.Message ?? "Login failed."));
        }

        var token = await _jwtService.GenerateTokenAsync(
            userId: loginResult.UserId!,
            email: request.Email,
            role: loginResult.UserRole,
            bpCode: loginResult.BPCode);

        _logger.LogInformation("User {UserId} logged in from {IP}", loginResult.UserId, remoteIp);

        return Ok(ApiResponse<AuthTokenResponse>.Ok(new AuthTokenResponse
        {
            Token = token,
            UserId = loginResult.UserId!,
            Email = request.Email,
            BPCode = loginResult.BPCode,
            BPName = loginResult.BPName,
            FirstName = loginResult.FirstName,
            LastName = loginResult.LastName,
            UserRole = loginResult.UserRole,
            UserRoleName = loginResult.UserRoleName,
            BPRoleName = loginResult.BPRoleName,
            UserAccessObjects = loginResult.UserAccessObjects,
            BPAccessObjects = loginResult.BPAccessObjects
        }));
    }

    /// <summary>
    /// Quick login by userId (development/internal use), returns JWT token
    /// </summary>
    [HttpPost("login/fast/{userId}")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthTokenResponse>>> FastLogin(string userId)
    {
        if (string.IsNullOrEmpty(userId))
        {
            return BadRequest(ApiResponse<AuthTokenResponse>.Fail("UserId is required."));
        }

        var remoteIp = _databaseService.GetClientIpAddress(HttpContext);

        var parameters = new Dictionary<string, object>
        {
            { "RemoteIP", remoteIp },
            { "APPID", "NewWWW" },
            { "UserID", userId }
        };

        var dataSet = await _databaseService.ExecuteStoredProcedureAsync("psp_CheckSAP", parameters);
        var loginResult = ParseLoginDataSet(dataSet);

        if (loginResult == null || loginResult.Status < 0)
        {
            return Unauthorized(ApiResponse<AuthTokenResponse>.Fail(
                loginResult?.Message ?? "Login failed."));
        }

        var token = await _jwtService.GenerateTokenAsync(
            userId: loginResult.UserId!,
            email: loginResult.UserId!, // UserId is the identifier for fast login
            role: loginResult.UserRole,
            bpCode: loginResult.BPCode);

        _logger.LogInformation("Fast login for user {UserId} from {IP}", userId, remoteIp);

        return Ok(ApiResponse<AuthTokenResponse>.Ok(new AuthTokenResponse
        {
            Token = token,
            UserId = loginResult.UserId!,
            BPCode = loginResult.BPCode,
            BPName = loginResult.BPName,
            FirstName = loginResult.FirstName,
            LastName = loginResult.LastName,
            UserRole = loginResult.UserRole,
            UserRoleName = loginResult.UserRoleName,
            BPRoleName = loginResult.BPRoleName,
            UserAccessObjects = loginResult.UserAccessObjects,
            BPAccessObjects = loginResult.BPAccessObjects
        }));
    }

    /// <summary>
    /// Get current user info from JWT claims
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public ActionResult<ApiResponse<AuthMeResponse>> Me()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        var bpCode = User.FindFirst("BPCode")?.Value;
        var scopes = User.FindAll("scope").Select(c => c.Value).ToList();

        if (string.IsNullOrEmpty(userId))
        {
            return Unauthorized(ApiResponse<AuthMeResponse>.Fail("Invalid token."));
        }

        return Ok(ApiResponse<AuthMeResponse>.Ok(new AuthMeResponse
        {
            UserId = userId,
            Email = email,
            Role = role,
            BPCode = bpCode,
            Scopes = scopes.Count > 0 ? scopes : null
        }));
    }

    private static LoginParsedResult? ParseLoginDataSet(System.Data.DataSet dataSet)
    {
        if (dataSet.Tables.Count == 0 || dataSet.Tables[0].Rows.Count == 0)
            return null;

        var row = dataSet.Tables[0].Rows[0];
        var status = int.Parse(row["ReturnCode"]?.ToString() ?? "-1");

        var result = new LoginParsedResult
        {
            Status = status,
            Message = row["Message"]?.ToString(),
            UserId = row["UserID"]?.ToString(),
            BPCode = row["BPCode"]?.ToString()
        };

        if (status >= 0)
        {
            result.FirstName = row["FirstName"]?.ToString();
            result.LastName = row["LastName"]?.ToString();
            result.BPName = row["BPName"]?.ToString();
            result.UserRole = row["UserRole"]?.ToString();
            result.UserRoleName = row["UserRoleName"]?.ToString();
            result.UserAccessObjects = row["UserAccessObjects"]?.ToString();
            result.BPRoleName = row["BPRoleName"]?.ToString();
            result.BPAccessObjects = row["BPAccessObjects"]?.ToString();
        }

        return result;
    }

    private class LoginParsedResult
    {
        public int Status { get; set; }
        public string? Message { get; set; }
        public string? UserId { get; set; }
        public string? BPCode { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? BPName { get; set; }
        public string? UserRole { get; set; }
        public string? UserRoleName { get; set; }
        public string? UserAccessObjects { get; set; }
        public string? BPRoleName { get; set; }
        public string? BPAccessObjects { get; set; }
    }
}

#region Auth Response Models

public class AuthTokenResponse
{
    public string Token { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? BPCode { get; set; }
    public string? BPName { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? UserRole { get; set; }
    public string? UserRoleName { get; set; }
    public string? BPRoleName { get; set; }
    public string? UserAccessObjects { get; set; }
    public string? BPAccessObjects { get; set; }
}

public class AuthMeResponse
{
    public string UserId { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? Role { get; set; }
    public string? BPCode { get; set; }
    public List<string>? Scopes { get; set; }
}

#endregion
