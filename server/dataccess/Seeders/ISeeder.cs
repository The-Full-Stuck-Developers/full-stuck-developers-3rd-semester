using System.Threading.Tasks;
using dataccess; 

namespace dataccess.Seeders;

public interface ISeeder
{
    Task SeedAsync(MyDbContext db);
}