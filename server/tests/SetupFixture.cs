using api.Etc;
using api.Services;
using dataccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace tests;

public sealed class SetupFixture : IDisposable
{
    public MyDbContext Ctx { get; }
    public ISeeder Seeder { get; }
    public IAuthService AuthService { get; }

    private readonly ServiceProvider _sp;

    public SetupFixture()
    {
        var services = new ServiceCollection();

        services.AddDbContext<MyDbContext>(o =>
            o.UseInMemoryDatabase(Guid.NewGuid().ToString()));

        services.AddLogging();

        services.AddScoped<
            dataccess.Repositories.IRepository<dataccess.Entities.User>,
            dataccess.Repositories.UserRepository>();

        services.AddScoped<
            Microsoft.AspNetCore.Identity.IPasswordHasher<dataccess.Entities.User>,
            Microsoft.AspNetCore.Identity.PasswordHasher<dataccess.Entities.User>>();

        services.AddScoped<IAuthService, AuthService>();
        
        services.AddSingleton(TimeProvider.System);

        services.AddScoped<ISeeder, SieveTestSeeder>();

        services.AddScoped<IEmailService, FakeEmailService>();

        services.AddSingleton(new api.Models.AppOptions
        {
            FrontendUrl = "https://frontend.test"
        });

        _sp = services.BuildServiceProvider();

        Ctx = _sp.GetRequiredService<MyDbContext>();
        Seeder = _sp.GetRequiredService<ISeeder>();
        AuthService = _sp.GetRequiredService<IAuthService>();
    }

    public void Dispose()
    {
        Ctx.Dispose();
        _sp.Dispose();
    }
}

internal sealed class FakeEmailService : IEmailService
{
    public Task SendPasswordResetEmail(string toEmail, string userName, string resetLink)
        => Task.CompletedTask;
}