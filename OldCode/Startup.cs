

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WebPush;

namespace FunTimePIE
{

    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;

        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
          //  Utility.DefaultConnectionString = Configuration.GetSection("ConnectionStrings")["HYTRestAPI"];


            // Add HttpClientFactory service
            services.AddHttpClient();

            services.AddControllers();
            
            services.AddCors(o => o.AddPolicy("AllowAll", b =>
        b.AllowAnyOrigin()
         .AllowAnyMethod()
         .AllowAnyHeader()));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI();

            }

            app.UseRouting();

            app.UseCors("AllowAll");      // before auth
            app.UseAuthentication();      // if used

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/_debug/routes", async context =>
                {
                    var routeEndpointDataSource = context.RequestServices.GetRequiredService<EndpointDataSource>();
                    var routes = routeEndpointDataSource.Endpoints.Select(e => e.DisplayName);
                    await context.Response.WriteAsync(string.Join("\n", routes));
                });
            });

        }
    }
}
