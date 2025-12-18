using dataccess.Repositories;

namespace tests.Helpers;

class InMemoryRepository<T>(IList<T> entities) : IRepository<T>
    where T : class
{
    public async Task Add(T entity)
    {
        entities.Add(entity);
    }

    public async Task Delete(T entity)
    {
        var reference = entities.Single((t) => (t as dynamic).Id == (entity as dynamic).Id);
        entities.Remove(reference);
    }

    public async Task Update(T entity)
    {
        await Delete(entity);
        await Add(entity);
    }

    public Task AddAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public Task<T?> GetAsync(Func<T, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public IQueryable<T> Query()
    {
        return entities.AsQueryable();
    }

    public Task UpdateAsync(T entity)
    {
        throw new NotImplementedException();
    }
}