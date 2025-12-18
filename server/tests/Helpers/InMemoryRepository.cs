using dataccess.Repositories;

namespace tests.Helpers;

public class InMemoryRepository<T>(IList<T> entities) : IRepository<T>
    where T : class
{
    public Task AddAsync(T entity)
    {
        entities.Add(entity);
        return Task.CompletedTask;
    }

    public Task DeleteAsync(T entity)
    {
        var reference = entities.Single(t => (t as dynamic).Id == (entity as dynamic).Id);
        entities.Remove(reference);
        return Task.CompletedTask;
    }

    public async Task UpdateAsync(T entity)
    {
        await DeleteAsync(entity);
        await AddAsync(entity);
    }

    public Task<T?> GetAsync(Func<T, bool> predicate)
        => Task.FromResult(entities.FirstOrDefault(predicate));

    public IQueryable<T> Query() => entities.AsQueryable();

    // (Optional) if your interface still includes these sync-ish ones, keep them:
    public Task Add(T entity) => AddAsync(entity);
    public Task Delete(T entity) => DeleteAsync(entity);
    public Task Update(T entity) => UpdateAsync(entity);
}