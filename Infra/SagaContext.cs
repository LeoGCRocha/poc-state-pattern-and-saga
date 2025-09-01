using Domain;
using Infra.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Infra;

public class SagaContext : DbContext
{
    public SagaContext(DbContextOptions<SagaContext> options) : base(options) {}
    
    public DbSet<SagaEntity> Sagas { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SagaConfiguration).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}