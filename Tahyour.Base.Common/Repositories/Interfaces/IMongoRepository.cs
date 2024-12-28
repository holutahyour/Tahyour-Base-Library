namespace Tahyour.Base.Common.Repositories.Interfaces;

public interface IMongoRepository<T> where T : class
{
    Task<T[]> AddEntitiesAsync(T[] entities);
    Task<T> CreateAsync(T entity);
    Task<IList<string>> DeleteAsync(Expression<Func<T, bool>> expression);
    Task<bool> DeleteAsync(Guid id);
    Task<IList<T>> GetAllAsync();
    Task<IList<T>> GetAllAsync(string search = null, string filter = null, int page = 1, int pageSize = 10);
    Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> expression);
    Task<T?> GetAsync(Expression<Func<T, bool>> expression);
    Task<T> GetByCodeAsync(string code);
    Task<T?> GetByIdAsync(Guid id);
    Task<bool> UpdateAsync(Guid id, T entity);
}