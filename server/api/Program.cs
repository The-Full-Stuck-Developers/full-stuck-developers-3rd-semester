using System.Text.Json.Serialization;
using api.Etc;
using api.Models;
using api.Security;
using api.Services;
using dataccess;
using dataccess.Entities;
using dataccess.Repositories;
using dataccess.Seeders;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Sieve.Models;
using Sieve.Services;

namespace api;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        ConfigureServices(builder, builder.Configuration);

        var app = builder.Build();

        if (args is [.., "setup", var defaultPassword])
        {
            SetupDatabase(app, defaultPassword);
            Environment.Exit(0);
        }

        await ConfigureApp(app);
        await app.RunAsync();
    }

    public static void SetupDatabase(WebApplication app, string defaultPassword)
    {
        using var scope = app.Services.CreateScope();

        var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();
        db.Database.Migrate();

        var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
        seeder.Seed(defaultPassword).Wait();
    }

    public static void ConfigureServices(WebApplicationBuilder builder, IConfiguration configuration)
    {
        var services = builder.Services;

        var appOptions = new AppOptions();
        configuration.GetSection("AppOptions").Bind(appOptions);
        services.AddSingleton(appOptions);

        services.AddDbContext<MyDbContext>(options =>
            options.UseNpgsql(appOptions.DefaultConnection)
                   .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));

        services.AddControllers().AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            opts.JsonSerializerOptions.MaxDepth = 128;
        });

        services.AddOpenApiDocument();
        
        services.AddCors(options =>
        {
            options.AddPolicy("FrontendCors", policy =>
            {
                policy.WithOrigins(
                        "http://localhost:5173",
                        "http://localhost:5174",
                        "https://deadpigeons.vercel.app"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                    // .AllowCredentials(); // only if you use cookies
            });
        });

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITokenService, JwtService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IGameService, GameService>();
        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped<DbSeeder>();
        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters =
                    JwtService.CreateValidationParams(builder.Configuration);
            });

        builder.Services.AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>();

        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy("IsAdmin", policy =>
                policy.Requirements.Add(new IsAdmin()));
        });

        services.Configure<SieveOptions>(options =>
        {
            options.CaseSensitive = false;
            options.DefaultPageSize = 10;
            options.MaxPageSize = 100;
        });

        services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();
    }

    private static async Task ConfigureApp(WebApplication app)
    {
        app.UseExceptionHandler();
        
        app.UseCors("FrontendCors");

        app.UseOpenApi();
        app.UseSwaggerUi();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.MapGet("/", [AllowAnonymous] () =>
            Results.Ok("DeadPigeons API is running"));

        await app.GenerateApiClientsFromOpenApi(
            "/../../client/src/core/generated-client.ts");

        if (app.Environment.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();

            await db.Database.MigrateAsync();

            if (!await db.Users.AnyAsync())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
                await seeder.Seed("hashed_password_here");
            }
        }

    }
}
