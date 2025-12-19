using dataccess;
using dataccess.Repositories;
using Microsoft.EntityFrameworkCore;

namespace tests.Helpers;

/// <summary>
/// Base repository implementation for testing that wraps DbContext
/// Use this for all entity types in your tests
/// </summary>
public class TestRepository<T> : IRepository<T> where T : class
{
    private readonly MyDbContext _context;
    private readonly DbSet<T> _dbSet;

    public TestRepository(MyDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetAsync(Func<T, bool> predicate)
    {
        return await Task.FromResult(_dbSet.FirstOrDefault(predicate));
    }

    public IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}