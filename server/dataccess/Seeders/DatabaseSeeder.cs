//class to run all seeders in correct order

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dataccess;
using dataccess.Seeders;

namespace dataccess.Seeders;

public static class DatabaseSeeder
{
    public static async Task SeedAsync(MyDbContext db)
    {
        var seeders = new List<ISeeder>
        {
            new UserSeeder(),
            new GameSeeder(),
            new TransactionSeeder()
        };

        foreach (var seeder in seeders)
        {
            await seeder.SeedAsync(db);
            Console.WriteLine(">>> Running DatabaseSeeder.SeedAsync");
        }
    }
}