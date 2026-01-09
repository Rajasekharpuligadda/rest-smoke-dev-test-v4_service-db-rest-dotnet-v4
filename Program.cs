using ServiceDbRestDotnetV4.Core.Exceptions;
using ServiceDbRestDotnetV4.Core.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;  
using Microsoft.EntityFrameworkCore;

namespace ServiceDbRestDotnetV4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            // Add secrets loader to configuration before building services
            builder.Configuration.AddSecretsLoader();
            
            builder.Services.InitApp();
            builder.Services.AddControllers(opt =>
            {
                opt.Filters.Add<ExceptionFilter>();
            });
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c => { });
            
            // Add health checks
            builder.Services.AddHealthChecks()
                .AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "ready" });
                
                
            // Configure PostgreSQL DbContext
            var postgresDbUsername = Environment.GetEnvironmentVariable("POSTGRES_DB_USERNAME");
            var postgresDbPassword = Environment.GetEnvironmentVariable("POSTGRES_DB_PASSWORD");
            var postgresHost = Environment.GetEnvironmentVariable("POSTGRES_HOST");
            var postgresDbName = Environment.GetEnvironmentVariable("POSTGRES_DB_NAME");

            var connectionString = $"Host={postgresHost};Port=5432;Database={postgresDbName};Username={postgresDbUsername};Password={postgresDbPassword}";
            builder.Services.AddDbContext<Core.Data.AppDbContext>(options =>
                options.UseNpgsql(connectionString));
            
            
            
            var app = builder.Build();
            
            // Startup logging
            var logger = app.Services.GetRequiredService<ILogger<Program>>();
            logger.LogInformation("ServiceDbRestDotnetV4 is starting...");
            
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            
            // Health check endpoints
            app.MapHealthChecks("/health/live", new HealthCheckOptions
            {
                Predicate = _ => false
            });
            app.MapHealthChecks("/health/ready", new HealthCheckOptions
            {
                Predicate = check => check.Tags.Contains("ready")
            });
            app.UsePathBase("/service-db-rest-dotnet-v4");
            app.UseRouting();
            app.MapControllers();
            app.Run();
        }
    }
}
