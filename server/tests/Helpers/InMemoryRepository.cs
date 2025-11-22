using dataccess.Repositories;

namespace tests.Helpers;

class InMemoryRespository<T>(IList<T> entities) : IRepository<T>
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

    public IQueryable<T> Query()
    {
        return entities.AsQueryable();
    }
}