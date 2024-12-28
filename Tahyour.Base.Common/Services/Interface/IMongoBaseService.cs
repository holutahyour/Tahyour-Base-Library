namespace Tahyour.Base.Common.Services.Interface;

public interface IMongoBaseService<T>
{
    Task<Result<TResponse>> CreateAsync<TResponse, TRequest>(TRequest request);
    Task<Result<IList<TResponse>>> GetAllAsync<TResponse>();
    Task<Result<dynamic>> GetAllAsync<TResponse>(string search = null, string filter = null, int page = 1, int pageSize = 10, string select = null);
    Task<Result<TResponse>> GetByIdAsync<TResponse>(Guid id);
    Task<Result<string>> ImportAsync<TRequest>(TRequest[] entities);
    Task<Result<bool>> RemoveAsync(Guid id);
    Task<Result<bool>> UpdateAsync<TRequest>(Guid id, TRequest entity);
}
