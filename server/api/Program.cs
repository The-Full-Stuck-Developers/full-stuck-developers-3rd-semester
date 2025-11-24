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

        ConfigureApp(app);

        await app.RunAsync();
    }

    public static void SetupDatabase(WebApplication app, string defaultPassword)
    {
        using (var scope = app.Services.CreateScope())
        {
            var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
            seeder.Seed(defaultPassword).Wait();
        }
    }

    public static void ConfigureServices(WebApplicationBuilder builder, IConfiguration configuration)
    {
        var services = builder.Services;

        // Bind AppOptions
        var appOptions = new AppOptions();
        configuration.GetSection("AppOptions").Bind(appOptions);
        services.AddSingleton(appOptions);

        //Database
        services.AddDbContext<MyDbContext>(options =>
            options
                .UseNpgsql(appOptions.DefaultConnection)
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        services.AddSingleton(TimeProvider.System);

        Console.WriteLine($"Connecting to DB: {appOptions.DefaultConnection}");

        //Repositories
        services.AddScoped<IRepository<User>, UserRepository>();

        //Controllers
        services.AddControllers().AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            opts.JsonSerializerOptions.MaxDepth = 128;
        });

        //OpenApi
        services.AddOpenApiDocument(config =>
        {
            // config.AddStringConstants(typeof(SieveConstants));
        });

        //CORS
        services.AddCors(options =>
        {
            options.AddPolicy("AllowLocalhost5173", policy =>
            {
                policy.WithOrigins("http://localhost:5173")
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        //Core Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher>();

        // Register Seeder
        services.AddScoped<DbSeeder>();
        //Exceptions
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.AddProblemDetails();

        // JWT Authentication
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JwtService.CreateValidationParams(builder.Configuration);
            });

        // Global Authorization
        services.AddScoped<IAuthorizationHandler, AdminAuthorizationHandler>();
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();

            options.AddPolicy("IsAdmin", policy =>
                policy.AddRequirements(new IsAdmin()));
        });

        //Sieve
        services.Configure<SieveOptions>(options =>
        {
            options.CaseSensitive = false;
            options.DefaultPageSize = 10;
            options.MaxPageSize = 100;
        });
        services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();
    }

    private async static void ConfigureApp(WebApplication app)
    {
        // Exception handling should come first
        app.UseExceptionHandler();

        // Enable CORS BEFORE authentication/authorization
        app.UseCors(config => config
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()
            .SetIsOriginAllowed(x => true));

        // Swagger/OpenAPI (can be before or after CORS/auth)
        app.MapScalarApiReference(options => options.OpenApiRoutePattern = "/swagger/v1/swagger.json");
        app.UseOpenApi();
        app.UseSwaggerUi();

        // Map controllers
        app.MapControllers();

        // Generate client (optional, can be after mapping controllers)
        app.GenerateApiClientsFromOpenApi("/../../client/src/core/generated-client.ts").GetAwaiter().GetResult();

        // Authentication and Authorization
        app.UseAuthentication();
        app.UseAuthorization();

        if (app.Environment.IsDevelopment())
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();

                await db.Database.MigrateAsync();
                await DatabaseSeeder.SeedAsync(db);
            }
        }
    }
}