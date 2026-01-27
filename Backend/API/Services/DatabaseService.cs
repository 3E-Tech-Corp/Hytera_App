using Microsoft.Data.SqlClient;
using System.Data;

namespace HyteraAPI.Services;

public class DatabaseService : IDatabaseService
{
    private readonly string _connectionString;

    public DatabaseService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<DataSet> ExecuteStoredProcedureAsync(string procedureName, Dictionary<string, object>? parameters = null)
    {
        var dataSet = new DataSet();

        using var connection = new SqlConnection(_connectionString);
        using var command = new SqlCommand(procedureName, connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 60
        };

        if (parameters != null)
        {
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
            }
        }

        using var adapter = new SqlDataAdapter(command);
        await connection.OpenAsync();
        adapter.Fill(dataSet);

        return dataSet;
    }

    public string GetClientIpAddress(HttpContext httpContext)
    {
        string? ipAddress = httpContext.Connection.RemoteIpAddress?.ToString();

        if (httpContext.Request.Headers.ContainsKey("X-Forwarded-For"))
        {
            ipAddress = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        }

        return ipAddress ?? "unknown";
    }
}
