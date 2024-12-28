namespace Tahyour.Base.Common.Repositories.Implementations;

public class MongoRepository<T> : IMongoRepository<T> where T : BaseEntity<Guid>
{
    private readonly IMongoCollection<T> mongoCollection;
    private readonly FilterDefinitionBuilder<T> filterDefinitionBuilder = Builders<T>.Filter;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public MongoRepository(IHttpContextAccessor httpContextAccessor, IMongoDatabase database, string collectionName)
    {
        mongoCollection = database.GetCollection<T>(collectionName);
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<T[]> AddEntitiesAsync(T[] entities)
    {
        if (entities == null) throw new ArgumentNullException(nameof(entities));

        foreach (var entity in entities)
        {
            entity.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "SYSTEM";
            entity.CreatedOn = DateTime.UtcNow;
        }

        await mongoCollection.InsertManyAsync(entities);

        return entities;
    }

    public async Task<T> CreateAsync(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        entity.CreatedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "SYSTEM";
        entity.CreatedOn = DateTime.UtcNow;

        await mongoCollection.InsertOneAsync(entity);

        return entity;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        FilterDefinition<T> filter = filterDefinitionBuilder.Eq(entity => entity.Id, id);

        await mongoCollection.DeleteOneAsync(filter);

        return true;
    }

    public async Task<IList<string>> DeleteAsync(Expression<Func<T, bool>> expression)
    {
        var entities = await GetAllAsync();

        var ids = entities.Select(e => e.Id.ToString()).ToList();

        await mongoCollection.DeleteManyAsync(expression);

        return ids;
    }

    public async Task<IList<T>> GetAllAsync()
    {
        return await mongoCollection.Find(filterDefinitionBuilder.Empty).ToListAsync();
    }

    public async Task<IList<T>> GetAllAsync(Expression<Func<T, bool>> expression)
    {
        return await mongoCollection.Find(expression).ToListAsync();
    }

    public async Task<IList<T>> GetAllAsync(string search = null, string filter = null, int page = 1, int pageSize = 10)
    {
        var query = await mongoCollection.Find(filterDefinitionBuilder.Empty).ToListAsync();

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

                    //query = query.Where(lambda);
                }
            }
            catch (Exception)
            {

                throw new Exception("provide a valid filter parameter");
            }

        }

        // Fetch data from the database
        var results = query;

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

    public async Task<T?> GetAsync(Expression<Func<T, bool>> expression)
    {
        return await mongoCollection.Find(expression).FirstOrDefaultAsync();
    }

    public async Task<T> GetByCodeAsync(string code)
    {
        FilterDefinition<T> filter = filterDefinitionBuilder.Eq(entity => entity.Code, code);

        return await mongoCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        FilterDefinition<T> filter = filterDefinitionBuilder.Eq(entity => entity.Id, id);

        return await mongoCollection.Find(filter).FirstOrDefaultAsync();
    }

    public async Task<bool> UpdateAsync(Guid id, T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        FilterDefinition<T> filter = filterDefinitionBuilder.Eq(existingEntity => existingEntity.Id, id);

        entity.LastModifiedBy = _httpContextAccessor.HttpContext?.User.Identity?.Name ?? "SYSTEM";
        entity.LastModifiedOn = DateTime.UtcNow;

        await mongoCollection.ReplaceOneAsync(filter, entity);

        return true;
    }
}


public class Item : BaseEntity<Guid>
{
    public string Name { get; set; }
}

public class ItemDTO
{
    public string Name { get; set; }
}
