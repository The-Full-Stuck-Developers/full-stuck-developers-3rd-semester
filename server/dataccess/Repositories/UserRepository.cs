using dataccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace dataccess.Repositories;

public class UserRepository(MyDbContext context) : BaseRepository<User>(context)
{
    protected override DbSet<User> Set => Context.Users;

    public override async Task DeleteAsync(User entity)
    {
        entity.DeletedAt = DateTime.UtcNow;
        await UpdateAsync(entity);
    }
}