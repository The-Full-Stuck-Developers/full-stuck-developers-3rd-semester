using api.Services;
using dataccess;
using dataccess.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Sieve.Services;
using Testcontainers.PostgreSql;

namespace tests;

public sealed class Startup : IDisposable
{
    private static readonly PostgreSqlContainer Db =
        new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    private static bool _started;

    public Startup()
    {
        if (_started) return;

        // Start container BEFORE DI resolves DbContext
        Db.StartAsync().GetAwaiter().GetResult();
        _started = true;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddDbContext<MyDbContext>(opts => opts.UseNpgsql(Db.GetConnectionString()));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<ISieveProcessor, SieveProcessor>();
        services.AddScoped<GameService>();
    }

    public void Dispose()
    {
        if (!_started) return;

        Db.DisposeAsync().AsTask().GetAwaiter().GetResult();
        _started = false;
    }
}