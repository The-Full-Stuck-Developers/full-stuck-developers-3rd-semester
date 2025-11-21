using System.Text.Json.Serialization;
using api.Etc;
using api.Services;
using dataccess;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Sieve.Models;
using Sieve.Services;
using dataccess.Seeders;

namespace api;

public class Program
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(TimeProvider.System);
        services.InjectAppOptions();
        services.AddMyDbContext();

        services.AddControllers().AddJsonOptions(opts =>
        {
            opts.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve;
            opts.JsonSerializerOptions.MaxDepth = 128;
        });

        services.AddOpenApiDocument(config => { config.AddStringConstants(typeof(SieveConstants)); });
        services.AddCors();
        services.AddScoped<IAuthService, AuthService>();
        //services.AddScoped<ISeeder, SieveTestSeeder>();
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

        ConfigureServices(builder.Services);

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
