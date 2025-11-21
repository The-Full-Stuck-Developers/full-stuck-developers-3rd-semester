using dataccess;                       
using Microsoft.EntityFrameworkCore;  

namespace api.Etc;

public static class DbExtension
{
    public static IServiceCollection AddMyDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
                             ?? throw new InvalidOperationException(
                                 "Connection string 'DefaultConnection' not found.");

        services.AddDbContext<MyDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        return services;
    }
}
