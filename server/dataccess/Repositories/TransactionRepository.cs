using Microsoft.EntityFrameworkCore;

namespace dataccess.Repositories;

public class TransactionRepository : IRepository<Transaction>
{
    private readonly MyDbContext _dbContext;
    private readonly DbSet<Transaction> _dbSet;

    public TransactionRepository(MyDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = dbContext.Set<Transaction>();
    }

    public async Task AddAsync(Transaction entity)
    {
        await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(Transaction entity)
    {
        //_dbSet.Remove(entity);
        entity.Status = TransactionStatus.Cancelled;
        _dbSet.Update(entity);
        
        await _dbContext.SaveChangesAsync();
    }

    public Task<Transaction?> GetAsync(Func<Transaction, bool> predicate)
    {
        // IMPORTANT:
        // Func<T, bool> cannot be translated to SQL,
        // so this evaluates in memory.
        // Query() should be preferred for database filters.
        var result = _dbSet.AsEnumerable().FirstOrDefault(predicate);
        return Task.FromResult(result);
    }

    public IQueryable<Transaction> Query()
    {
        return _dbSet.AsQueryable();
    }

    public async Task UpdateAsync(Transaction entity)
    {
        _dbSet.Update(entity);
        await _dbContext.SaveChangesAsync();
    }
}
