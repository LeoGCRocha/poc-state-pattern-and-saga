using Infra;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace OrchestratorIntegration.Fixtures;

public class OrchestratorWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Remove all application related service
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var testSettings = new Dictionary<string, string>()
            {
                ["ConnectionStrings:DefaultConnectionTest"] = "Host=localhost;Port=5450;Database=sagacontexttest;Username=postgres;Password=postgres"
            };
            config.AddInMemoryCollection(testSettings!); 
        });
        
        builder.ConfigureServices((context, services) =>
        {
            var dbContext =
                services.SingleOrDefault(d => d.ServiceType == typeof(IDbContextOptionsConfiguration<SagaContext>));

            if (dbContext is not null)
                services.Remove(dbContext);

            var contextDescription = services.SingleOrDefault(d => d.ServiceType == typeof(SagaContext));

            if (contextDescription is not null)
                services.Remove(contextDescription);

            services.AddDbContext<SagaContext>(options =>
            {
                var connectionString = context.Configuration.GetConnectionString("DefaultConnectionTest");
                options.UseNpgsql(connectionString);
            });
        });

        builder.UseEnvironment("Development");
    }
}