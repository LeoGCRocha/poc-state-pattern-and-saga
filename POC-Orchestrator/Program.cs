using Infra;
using Refit;
using Domain;
using Infra.Repository;
using Microsoft.EntityFrameworkCore;
using Application.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ISagaRepository, SagaRepository>();
builder.Services.AddScoped<ISimpleApiService, SimpleApiService>();

builder.Services.AddRefitClient<IApiClient>()
    .ConfigureHttpClient(c => c.BaseAddress = new Uri("http://localhost:5155/start"));

builder.Services.AddDbContext<SagaContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    using var scope = app.Services.CreateScope();
    var database = scope.ServiceProvider.GetRequiredService<SagaContext>();
    database.Database.EnsureCreated();
}

app.MapControllers();
app.Run();
public partial class Program { }