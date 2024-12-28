namespace Tahyour.Base.Common.Repositories.Interfaces;

public interface IMSSQLRepository<T, I> where T : class
{
    Task<T[]> AddEntitiesAsync(T[] entities);
    Task<T> CreateAsync(T entity);
    Task<bool> DeleteAsync(I id);
    Task<IList<string>> DeleteAsync(Expression<Func<T, bool>> expression);
    Task<IList<T>> GetAllAsync();
    Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> expression);
    Task<T> GetByCodeAsync(string code);
    Task<T?> GetByIdAsync(I id);
    Task<T?> GetAsync(Expression<Func<T, bool>> expression);
    Task<bool> UpdateAsync(I id, T entity);
    Task<IList<T>> GetAllAsync(string search = null, string filter = null, int page = 1, int pageSize = 10);
}
