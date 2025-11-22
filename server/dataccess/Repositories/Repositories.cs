using dataccess;
using dataccess.Entities;
using dataccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace dataccess.Repositories;


public class UserRepository(MyDbContext context) : BaseRepository<User>(context)
{
    protected override DbSet<User> Set => Context.Users;
}

public abstract class BaseRepository<T>(MyDbContext context) : IRepository<T>
    where T : class
{
    protected MyDbContext Context => context;
    protected abstract DbSet<T> Set { get; }

    public async Task Add(T entity)
    {
        await Set.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public async Task Delete(T entity)
    {
        Set.Remove(entity);
        await context.SaveChangesAsync();
    }

    public T? Get(Func<T, bool> predicate)
    {
        return Set.Where(predicate).SingleOrDefault();
    }

    public IQueryable<T> Query()
    {
        return Set;
    }

    public async Task Update(T entity)
    {
        Set.Update(entity);
        await context.SaveChangesAsync();
    }
}