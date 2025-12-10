using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace dataccess;

public class MyDbContextFactory : IDesignTimeDbContextFactory<MyDbContext>
{
    public MyDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<MyDbContext>();
        
        var connectionString =
            "Host=127.0.0.1;Port=5433;Database=postgres;Username=postgres;Password=postgres";

        optionsBuilder.UseNpgsql(connectionString);

        return new MyDbContext(optionsBuilder.Options);
    }
}