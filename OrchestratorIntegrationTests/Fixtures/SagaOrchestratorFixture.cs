using Infra;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Respawn;
using Respawn.Graph;

namespace OrchestratorIntegration.Fixtures;

public class SagaOrchestratorFixture : IAsyncDisposable
{
    private readonly OrchestratorWebApplicationFactory<Program> _factory;
    private readonly IServiceScope _scope;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private Respawner? _respawner;
    
    public SagaOrchestratorFixture()
    {
        _factory = new OrchestratorWebApplicationFactory<Program>();
        _serviceScopeFactory = _factory.Services.GetRequiredService<IServiceScopeFactory>();
        _scope = _serviceScopeFactory.CreateScope();

        try
        {
            _scope.ServiceProvider
                .GetRequiredService<SagaContext>()
                .Database.EnsureCreated();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

    }   

    public async Task InitializeAsync()
    {
        var configuration = _factory.Services.GetRequiredService<IConfiguration>();
        var connectionString = configuration.GetConnectionString("DefaultConnectionTest");
        
        await using var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync();
        
        using var scope = _factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<SagaContext>();
        await dbContext.Database.EnsureCreatedAsync();
        
        _respawner = await Respawner.CreateAsync(conn, new RespawnerOptions()
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = [
                "public"
            ]
        });
        await _respawner.ResetAsync(conn);

    }

    public SagaContext GetDbContext()
    {
        return _scope.ServiceProvider.GetRequiredService<SagaContext>();
    }
    
    public async ValueTask DisposeAsync()
    {
        await _factory.DisposeAsync();
    }
}