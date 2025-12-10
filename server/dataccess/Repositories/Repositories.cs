using dataccess;
using dataccess.Entities;
using dataccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace dataccess.Repositories;

using dataccess;
using dataccess.Entities;
using dataccess.Repositories;
using Microsoft.EntityFrameworkCore;

public class GameRepository(MyDbContext context) : BaseRepository<Game>(context)
{
    protected override DbSet<Game> Set => Context.Games;
}

public abstract class BaseRepository<T>(MyDbContext context) : IRepository<T>
    where T : class
{
    protected MyDbContext Context => context;
    protected abstract DbSet<T> Set { get; }

    public async Task AddAsync(T entity)
    {
        await Set.AddAsync(entity);
        await context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        Set.Remove(entity);
        await context.SaveChangesAsync();
    }

    public async Task<T?> GetAsync(Func<T, bool> predicate)
    {
        return await Task.FromResult(Set.Where(predicate).SingleOrDefault());
    }

    public IQueryable<T> Query()
    {
        return Set.AsNoTracking();
    }

    public async Task UpdateAsync(T entity)
    {
        Set.Update(entity);
        await context.SaveChangesAsync();
    }
}