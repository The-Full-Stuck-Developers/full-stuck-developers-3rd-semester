using System.Text.Json.Serialization;
using api.Etc;
using api.Services;
using dataccess;
using dataccess.Seeders;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Sieve.Models;
using Sieve.Services;

namespace api;

public class Program
{
    public static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton(TimeProvider.System);
        services.InjectAppOptions();

        var connectionString = configuration.GetConnectionString("DefaultConnection");
        Console.WriteLine($"Connecting to DB: {connectionString}");

        services.AddDbContext<MyDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddControllers().AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            opts.JsonSerializerOptions.MaxDepth = 128;
        });

        services.AddOpenApiDocument(config =>
        {
            config.AddStringConstants(typeof(SieveConstants));
        });

        services.AddCors();
        services.AddScoped<IAuthService, AuthService>();
        services.AddExceptionHandler<GlobalExceptionHandler>();

        services.Configure<SieveOptions>(options =>
        {
            options.CaseSensitive = false;
            options.DefaultPageSize = 10;
            options.MaxPageSize = 100;
        });

        services.AddScoped<ISieveProcessor, ApplicationSieveProcessor>();
    }

    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder.Services, builder.Configuration);

        var app = builder.Build();

        app.UseExceptionHandler(config => { });
        app.UseOpenApi();
        app.UseSwaggerUi();

        app.MapScalarApiReference(options =>
            options.OpenApiRoutePattern = "/swagger/v1/swagger.json"
        );

        app.UseCors(config => config
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowAnyOrigin()
            .SetIsOriginAllowed(_ => true));

        app.MapControllers();

        app.GenerateApiClientsFromOpenApi("/../../client/src/core/generated-client.ts")
            .GetAwaiter()
            .GetResult();

        if (app.Environment.IsDevelopment())
        {
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<MyDbContext>();

                await db.Database.MigrateAsync();
                await DatabaseSeeder.SeedAsync(db);
            }
        }

        app.Run();
    }
}
