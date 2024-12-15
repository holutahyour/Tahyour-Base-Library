namespace Tahyour.Base.Common.Services.Interface;

public interface IBaseService<TEntity, TId>
{
    Task<Result<TResponse>> CreateAsync<TResponse, TRequest>(TRequest request);
    Task<Result<IList<TResponse>>> GetAllAsync<TResponse>();
    Task<Result<dynamic>> GetAllAsync<TResponse>(string search = null, string filter = null, int page = 1, int pageSize = 10, string select = null);
    Task<Result<TResponse>> GetByIdAsync<TResponse>(TId id);
    Task<Result<string>> ImportAsync<TRequest>(TRequest[] entities);
    Task<Result<bool>> RemoveAsync(TId id);
    Task<Result<bool>> UpdateAsync<TRequest>(TId id, TRequest entity);
}
