using System.Data;

namespace HyteraAPI.Services;

public interface IDatabaseService
{
    Task<DataSet> ExecuteStoredProcedureAsync(string procedureName, Dictionary<string, object>? parameters = null);
    string GetClientIpAddress(HttpContext httpContext);
}
