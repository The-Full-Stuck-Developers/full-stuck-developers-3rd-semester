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
            config.AddStringConstants(typeof(SieveConstants));
        });

        //CORS
        services.AddCors();

        //Core Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPasswordHasher<User>, PasswordHasher>();
        services.AddScoped<ITokenService, JwtService>();

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
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JwtService.CreateValidationParams(builder.Configuration);
                //Debugging
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        Console.WriteLine(context.Exception.Message);
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("Token validated successfully.");
                        return Task.CompletedTask;
                    },
                };
            });

        // Global Authorization
        services.AddAuthorization(options =>
        {
            options.FallbackPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
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
        //Scalar and Swagger
        app.MapScalarApiReference(options => options.OpenApiRoutePattern = "/swagger/v1/swagger.json");
        app.UseOpenApi();
        app.UseSwaggerUi();

        //Middleware
        app.UseExceptionHandler();
        app.UseAuthentication();
        app.UseAuthorization();

        //Cors
        app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().SetIsOriginAllowed(x => true));

        //Controllers
        app.MapControllers();

        //Client generation
        app.GenerateApiClientsFromOpenApi("/../../client/src/core/generated-client.ts").GetAwaiter().GetResult();

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