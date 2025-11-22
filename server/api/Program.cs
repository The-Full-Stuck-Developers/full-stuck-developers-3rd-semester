using System.Text.Json.Serialization;
using api.Etc;
using api.Security;
using api.Services;
using dataccess;
using dataccess.Entities;
using dataccess.Repositories;
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
        ConfigureServices(builder);
        
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

    public static void ConfigureServices(WebApplicationBuilder builder)
    {
        var services = builder.Services;
        
        //Database
        services.AddDbContext<MyDbContext>(options =>
            options
                .UseNpgsql(builder.Configuration.GetConnectionString("Db"))
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking));
        services.AddSingleton(TimeProvider.System);
        services.InjectAppOptions(builder.Configuration);
        services.AddMyDbContext();
        
        
        //Repositories
        services.AddScoped<IRepository<User>, UserRepository>();

       
        //Controllers
        services.AddControllers().AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
            opts.JsonSerializerOptions.MaxDepth = 128;
        });
        
        //OpenApi
        services.AddOpenApiDocument(config => { config.AddStringConstants(typeof(SieveConstants)); });
       
        //CORS
        services.AddCors();
        
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
    

    private static void ConfigureApp(WebApplication app)
    {

        //Middleware
        app.UseExceptionHandler();
        app.UseAuthentication();
        app.UseAuthorization();

        //Scalar and Swagger
        app.UseOpenApi();
        app.UseSwaggerUi();
        app.MapScalarApiReference(options => options.OpenApiRoutePattern = "/swagger/v1/swagger.json"
        );

        //Cors
        app.UseCors(config => config.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().SetIsOriginAllowed(x => true));

        //Controllers
        app.MapControllers();

        //Client generation
        app.GenerateApiClientsFromOpenApi("/../../client/src/core/generated-client.ts").GetAwaiter().GetResult();

    }
}