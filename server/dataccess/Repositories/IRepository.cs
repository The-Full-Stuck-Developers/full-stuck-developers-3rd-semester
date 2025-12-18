namespace dataccess.Repositories;

public interface IRepository<T> where T : class
{
    Task AddAsync(T entity);
    Task DeleteAsync(T entity);
    Task<T?> GetAsync(Func<T, bool> predicate);
    IQueryable<T> Query();
    Task UpdateAsync(T entity);
}
