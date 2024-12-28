namespace Tahyour.Base.Common.Services.Implementation;

public class MongoBaseService<T> : IMongoBaseService<T>
     where T : BaseEntity<Guid>
{
    private readonly IMongoRepository<T> _baseRepository;
    private readonly IMongoRepository<AuditLog> _auditLogRepository;
    private readonly IMapper _mapper;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MongoBaseService(
        IMongoRepository<T> baseRepository,
        IMongoRepository<AuditLog> auditLogRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
    )
    {
        _baseRepository = baseRepository;
        _auditLogRepository = auditLogRepository;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }

    public virtual async Task<Result<TResponse>> CreateAsync<TResponse, TRequest>(TRequest request)
    {
        var result = new Result<TResponse>(false);

        try
        {
            var entity = _mapper.Map<T>(request);

            if (entity != null)
            {
                entity.Code ??= RandomGenerator.RandomString(10);
            }

            var response = await _baseRepository.CreateAsync(entity);

            var auditLog = new AuditLog(_httpContextAccessor, "Create", typeof(T).Name, null, JsonSerializer.Serialize(entity));

            await _auditLogRepository.CreateAsync(auditLog);

            if (response == null)
            {
                result.SetError($"{typeof(T).Name} not created", $"{typeof(T).Name} not created");
            }
            else
            {
                result.SetSuccess(_mapper.Map<TResponse>(response), $"{typeof(T).Name} created successfully!");
            }
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), $"Error while creating {typeof(T).Name}");
        }

        return result;
    }

    public virtual async Task<Result<IList<TResponse>>> GetAllAsync<TResponse>()
    {
        var result = new Result<IList<TResponse>>(false);

        try
        {
            var response = await _baseRepository.GetAllAsync();
            result.SetSuccess(_mapper.Map<IList<TResponse>>(response), "Retrieved successfully.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while retrieving records.");
        }

        return result;
    }

    public virtual async Task<Result<dynamic>> GetAllAsync<TResponse>(
        string search = null,
        string filter = null,
        int page = 1,
        int pageSize = 10,
        string select = null)
    {
        Result<dynamic> result = new(false);

        try
        {
            var response = await _baseRepository.GetAllAsync(search, filter, page, pageSize);

            var responseDTO = _mapper.Map<IList<TResponse>>(response);

            // Selecting specific properties
            if (!string.IsNullOrEmpty(select))
            {
                var selectedProperties = select.Split(',', StringSplitOptions.TrimEntries);
                var selectedData = responseDTO.Select(s => BaseServiceHelper.SelectProperties(s, selectedProperties)).ToList();


                result.SetSuccess(selectedData, "Retrieved Successfully.");

                return result;
            }

            result.SetSuccess(responseDTO, "Retrieved Successfully.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.Message, $"Error while retrieving {typeof(T).Name}");

            return result;
        }
        return result;
    }

    public virtual async Task<Result<TResponse>> GetByIdAsync<TResponse>(Guid id)
    {
        var result = new Result<TResponse>(false);

        try
        {
            var response = await _baseRepository.GetByIdAsync(id);
            result.SetSuccess(_mapper.Map<TResponse>(response), "Retrieved successfully.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while retrieving record.");
        }

        return result;
    }

    public virtual async Task<Result<bool>> RemoveAsync(Guid id)
    {
        var result = new Result<bool>(false);

        try
        {
            var existingEntity = await _baseRepository.GetByIdAsync(id);
            var oldValues = JsonSerializer.Serialize(existingEntity);

            var response = await _baseRepository.DeleteAsync(id);


            if (!response)
            {
                result.SetError($"{typeof(T).Name} not deleted", $"{typeof(T).Name} with Id {id} not deleted");
            }
            else
            {
                var auditLog = new AuditLog(_httpContextAccessor, "Delete", typeof(T).Name, oldValues);

                await _auditLogRepository.CreateAsync(auditLog);

                result.SetSuccess(response, $"{typeof(T).Name} with Id {id} deleted successfully!");
            }
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), $"Error while removing {typeof(T).Name}");
        }

        return result;
    }

    public virtual async Task<Result<bool>> UpdateAsync<TRequest>(Guid id, TRequest request)
    {
        var result = new Result<bool>(false);

        try
        {
            var existingEntity = await _baseRepository.GetByIdAsync(id);
            var oldValues = JsonSerializer.Serialize(existingEntity);


            if (existingEntity == null)
            {
                result.SetError($"{typeof(T).Name} not updated", $"Record with Id {id} not found.");
                return result;
            }

            _mapper.Map(request, existingEntity);

            await _baseRepository.UpdateAsync(id, existingEntity);

            var auditLog = new AuditLog(_httpContextAccessor, "Update", typeof(T).Name, oldValues, JsonSerializer.Serialize(existingEntity));

            await _auditLogRepository.CreateAsync(auditLog);


            result.SetSuccess(true, $"{typeof(T).Name} with Id {id} updated successfully.");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), "Error while updating record.");
        }

        return result;
    }

    public virtual async Task<Result<string>> ImportAsync<TRequest>(TRequest[] requests)
    {
        var result = new Result<string>(false);

        try
        {
            if (requests == null || requests.Length == 0)
            {
                result.SetError($"{typeof(T).Name}s not imported", "No data provided for import.");
                return result;
            }

            var entities = _mapper.Map<T[]>(requests);

            foreach (var entity in entities)
            {
                if (entity != null)
                {
                    entity.Code ??= RandomGenerator.RandomString(10);
                }
            }

            await _baseRepository.AddEntitiesAsync(entities);

            result.SetSuccess("Success", $"{typeof(T).Name}s imported successfully!");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), $"Error while importing {typeof(T).Name}s");
        }

        return result;
    }


}
