using HyteraAPI.Data;
using HyteraAPI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add HttpClient factory
builder.Services.AddHttpClient();

// Add DbContext
builder.Services.AddDbContext<HyteraDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add custom services
builder.Services.AddScoped<IDatabaseService, DatabaseService>();
builder.Services.AddScoped<IAssetService, AssetService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

// Debug routes endpoint (development only)
if (app.Environment.IsDevelopment())
{
    app.MapGet("/_debug/routes", (EndpointDataSource endpointDataSource) =>
    {
        var routes = endpointDataSource.Endpoints.Select(e => e.DisplayName);
        return string.Join("\n", routes);
    });
}

app.Run();
