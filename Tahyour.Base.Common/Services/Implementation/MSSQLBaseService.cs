namespace Tahyour.Base.Common.Services.Implementation;

public abstract class MSSQLBaseService<TEntity, TId> : IMSSQLBaseService<TEntity, TId>
     where TEntity : BaseEntity<TId>
{
    private readonly IMSSQLRepository<TEntity, TId> _baseRepository;
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    protected MSSQLBaseService(
        IMSSQLRepository<TEntity, TId> baseRepository,
        IApplicationDbContext context,
        IMapper mapper
    )
    {
        _baseRepository = baseRepository;
        _context = context;
        _mapper = mapper;
    }

    public virtual async Task<Result<TResponse>> CreateAsync<TResponse, TRequest>(TRequest request)
    {
        var result = new Result<TResponse>(false);

        try
        {
            var entity = _mapper.Map<TEntity>(request);

            if (entity != null)
            {
                entity.Code ??= RandomGenerator.RandomString(10);
            }

            var response = await _baseRepository.CreateAsync(entity);
            await _context.SaveChangesAsync();

            if (response == null)
            {
                result.SetError($"{typeof(TEntity).Name} not created", $"{typeof(TEntity).Name} not created");
            }
            else
            {
                result.SetSuccess(_mapper.Map<TResponse>(response), $"{typeof(TEntity).Name} created successfully!");
            }
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), $"Error while creating {typeof(TEntity).Name}");
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
            result.SetError(ex.Message, $"Error while retrieving {typeof(TEntity).Name}");

            return result;
        }
        return result;
    }

    public virtual async Task<Result<TResponse>> GetByIdAsync<TResponse>(TId id)
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

    public virtual async Task<Result<bool>> RemoveAsync(TId id)
    {
        var result = new Result<bool>(false);

        try
        {
            var response = await _baseRepository.DeleteAsync(id);
            await _context.SaveChangesAsync();

            if (!response)
            {
                result.SetError($"{typeof(TEntity).Name} not deleted", $"{typeof(TEntity).Name} with Id {id} not deleted");
            }
            else
            {
                result.SetSuccess(response, $"{typeof(TEntity).Name} with Id {id} deleted successfully!");
            }
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), $"Error while removing {typeof(TEntity).Name}");
        }

        return result;
    }

    public virtual async Task<Result<bool>> UpdateAsync<TRequest>(TId id, TRequest request)
    {
        var result = new Result<bool>(false);

        try
        {
            var existingEntity = await _baseRepository.GetByIdAsync(id);

            if (existingEntity == null)
            {
                result.SetError($"{typeof(TEntity).Name} not updated", $"Record with Id {id} not found.");
                return result;
            }

            _mapper.Map(request, existingEntity);
            await _context.SaveChangesAsync();

            result.SetSuccess(true, $"{typeof(TEntity).Name} with Id {id} updated successfully.");
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
                result.SetError($"{typeof(TEntity).Name}s not imported", "No data provided for import.");
                return result;
            }

            var entities = _mapper.Map<TEntity[]>(requests);

            foreach (var entity in entities)
            {
                if (entity != null)
                {
                    entity.Code ??= RandomGenerator.RandomString(10);
                }
            }

            await _baseRepository.AddEntitiesAsync(entities);
            await _context.SaveChangesAsync();

            result.SetSuccess("Success", $"{typeof(TEntity).Name}s imported successfully!");
        }
        catch (Exception ex)
        {
            result.SetError(ex.ToString(), $"Error while importing {typeof(TEntity).Name}s");
        }

        return result;
    }
}
