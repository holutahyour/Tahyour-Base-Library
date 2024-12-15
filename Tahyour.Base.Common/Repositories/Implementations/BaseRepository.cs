namespace Tahyour.Base.Common.Repositories.Implementations;

public abstract class BaseRepository<T, I> : IBaseRepository<T, I>
    where T : BaseEntity<I>
{
    private readonly IApplicationDbContext _context;

    protected BaseRepository(IApplicationDbContext context) => _context = context ?? throw new ArgumentNullException(nameof(context));

    #region CRUD Operations

    public virtual async Task<IList<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }


    public async Task<IList<T>> GetAllAsync(
        string search = null,
        string filter = null,
        int page = 1,
        int pageSize = 10)
    {
        IQueryable<T> query = _context.Set<T>();

        // Dynamically include all navigation properties
        var navigationProperties = typeof(T).GetProperties()
            .Where(prop => typeof(IEnumerable<object>).IsAssignableFrom(prop.PropertyType) ||
                           (prop.PropertyType.IsClass && prop.PropertyType != typeof(string)));

        foreach (var navigationProperty in navigationProperties)
        {
            query = query.Include(navigationProperty.Name);
        }

        // Apply filtering based on the search term
        if (!string.IsNullOrEmpty(filter))
        {
            try
            {
                var filterParts = filter.Split('=', StringSplitOptions.TrimEntries);
                if (filterParts.Length == 2)
                {
                    var propertyName = filterParts[0];
                    var propertyValue = filterParts[1];

                    var parameter = Expression.Parameter(typeof(T), nameof(T));
                    var property = Expression.Property(parameter, propertyName);
                    var constant = Expression.Constant(propertyValue, typeof(string));

                    // Create an Equals expression
                    var equalsExpression = Expression.Equal(property, constant);

                    // Create the lambda: result => result.PropertyName == "propertyValue"
                    var lambda = Expression.Lambda<Func<T, bool>>(equalsExpression, parameter);

                    query = query.Where(lambda);
                }
            }
            catch (Exception)
            {

                throw new Exception("provide a valid filter parameter");
            }

        }

        // Fetch data from the database
        var results = await query.ToListAsync();

        // Apply search across all properties in-memory
        if (!string.IsNullOrEmpty(search))
        {
            try
            {
                var resultProperties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                results = results
                    .Where(result =>
                        resultProperties.Any(prop =>
                        {
                            var value = prop.GetValue(result)?.ToString();
                            return value != null && value.Contains(search, StringComparison.OrdinalIgnoreCase);
                        }))
                    .ToList();
            }
            catch (Exception ex)
            {

                throw new Exception("provide a valid search parameter");
            }

        }

        // Total data count (after filtering and searching)
        var totalCount = results.Count;

        // Pagination

        if (page != 0 && pageSize != 0)
        {
            results = results
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        }


        return results;
    }


    public virtual async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> expression)
    {
        return await _context.Set<T>().Where(expression).ToListAsync();
    }

    public virtual async Task<T?> GetAsync(Expression<Func<T, bool>> expression)
    {
        return await _context.Set<T>().FirstOrDefaultAsync(expression);
    }

    public virtual async Task<T?> GetByIdAsync(I id)
    {
        ArgumentValidatorHelpers.ValidateArgument(id, nameof(id));
        return await _context.Set<T>().FirstOrDefaultAsync(x => x.Id.Equals(id));
    }

    public virtual async Task<T?> GetByCodeAsync(string code)
    {
        ArgumentValidatorHelpers.ValidateStringArgument(code, nameof(code));
        return await _context.Set<T>().FindAsync(code);
    }

    public virtual async Task<T> CreateAsync(T entity)
    {
        ArgumentValidatorHelpers.ValidateArgument(entity, nameof(entity));
        await _context.Set<T>().AddAsync(entity);
        return entity;
    }

    public virtual async Task<bool> UpdateAsync(I id, T entity)
    {
        ArgumentValidatorHelpers.ValidateArgument(entity, nameof(entity));

        try
        {
            _context.Set<T>().Update(entity);
            return true;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while updating the entity.", ex);
        }
    }

    public virtual async Task<T[]> AddEntitiesAsync(T[] entities)
    {
        ArgumentValidatorHelpers.ValidateArgument(entities, nameof(entities));

        try
        {
            await _context.Set<T>().AddRangeAsync(entities);
            return entities;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("An error occurred while adding entities.", ex);
        }
    }

    public virtual async Task<bool> DeleteAsync(I id)
    {
        ArgumentValidatorHelpers.ValidateArgument(id, nameof(id));

        var entity = await GetByIdAsync(id);
        if (entity == null) throw new KeyNotFoundException("Entity not found.");

        _context.Set<T>().Remove(entity);
        return true;
    }

    public virtual async Task<IList<T>> DeleteAsync(Expression<Func<T, bool>> expression)
    {
        ArgumentValidatorHelpers.ValidateArgument(expression, nameof(expression));

        return await DeleteEntitiesAsync(expression);
    }

    #endregion

    #region Helper Methods

    private async Task<IList<T>> GetFilteredEntitiesAsync(Dictionary<string, string> fieldValues)
    {
        ArgumentValidatorHelpers.ValidateArgument(fieldValues, nameof(fieldValues));

        var lambda = ExpressionGenerator.GenerateLambda<T>(fieldValues);
        return await _context.Set<T>().Where(lambda).ToListAsync();
    }

    private async Task<IList<T>> DeleteEntitiesAsync(Expression<Func<T, bool>> expression)
    {
        var entitiesToDelete = await _context.Set<T>().Where(expression).ToListAsync();
        if (entitiesToDelete.Count == 0) throw new ArgumentException("No matching records found.");

        _context.Set<T>().RemoveRange(entitiesToDelete);
        await _context.SaveChangesAsync();

        return entitiesToDelete;
    }

    #endregion
}
