# README: How to Inherit and Override Repository, Service, and Presentation Layers

This guide explains how to extend the provided `MongoRepository`, `MongoBaseService`, and `MongoBaseController` classes to create specific implementations for your application.

---

## 1. **Repository Layer**
The repository layer is responsible for interacting with the database. To create a custom repository for a specific entity, you can inherit from the `MongoRepository<T>` class.

### Steps to Inherit and Override the Repository

1. **Create a Custom Repository Class**
   ```csharp
   public class ItemRepository : MongoRepository<Item>
   {
       public ItemRepository(IHttpContextAccessor httpContextAccessor, IMongoDatabase database)
           : base(httpContextAccessor, database, "Items")
       {
       }

       // Add custom methods if needed
       public async Task<IList<Item>> GetItemsByNameAsync(string name)
       {
           var filter = Builders<Item>.Filter.Eq(i => i.Name, name);
           return await mongoCollection.Find(filter).ToListAsync();
       }
   }
   ```

2. **Override Methods (Optional)**
   If you need to customize existing methods like `CreateAsync` or `DeleteAsync`, override them in your custom class.
   ```csharp
   public override async Task<Item> CreateAsync(Item entity)
   {
       entity.Name = entity.Name.ToUpper();
       return await base.CreateAsync(entity);
   }
   ```

---

## 2. **Service Layer**
The service layer handles business logic and communicates between the repository and the presentation layers. To create a custom service for an entity, inherit from `MongoBaseService<T>`.

### Steps to Inherit and Override the Service

1. **Create a Custom Service Class**
   ```csharp
   public class ItemService : MongoBaseService<Item>
   {
       public ItemService(
           IMongoRepository<Item> baseRepository,
           IMongoRepository<AuditLog> auditLogRepository,
           IMapper mapper,
           IHttpContextAccessor httpContextAccessor)
           : base(baseRepository, auditLogRepository, mapper, httpContextAccessor)
       {
       }

       // Add custom methods if needed
       public async Task<Result<IList<ItemDTO>>> GetItemsByCustomFilterAsync(string filter)
       {
           // Custom business logic
           var response = await _baseRepository.GetAllAsync(i => i.Name.Contains(filter));
           return new Result<IList<ItemDTO>>(true, _mapper.Map<IList<ItemDTO>>(response), "Filtered items retrieved.");
       }
   }
   ```

2. **Override Methods (Optional)**
   Customize methods like `CreateAsync` or `UpdateAsync` by overriding them.
   ```csharp
   public override async Task<Result<ItemDTO>> CreateAsync<ItemDTO, ItemRequest>(ItemRequest request)
   {
       var entity = _mapper.Map<Item>(request);
       entity.Name = entity.Name.Trim();
       var response = await base.CreateAsync<ItemDTO, ItemRequest>(request);
       return response;
   }
   ```

---

## 3. **Presentation Layer**
The presentation layer provides API endpoints. To create a custom controller for an entity, inherit from `MongoBaseController<T, TResponse>`.

### Steps to Inherit and Override the Controller

1. **Create a Custom Controller Class**
   ```csharp
   [Route("api/[controller]")]
   [ApiController]
   public class ItemController : MongoBaseController<Item, ItemDTO>
   {
       public ItemController(IMongoBaseService<Item> service) : base(service)
       {
       }

       // Add custom endpoints if needed
       [HttpGet("by-name/{name}")]
       public async Task<ActionResult> GetItemsByName(string name)
       {
           var response = await _service.GetAllAsync<ItemDTO>(filter: $"Name={name}");
           return Ok(response);
       }
   }
   ```

2. **Override Methods (Optional)**
   Customize methods like `GetAllAsync` by overriding them.
   ```csharp
   [HttpGet]
   public override async Task<ActionResult> GetAllAsync(string search = null, string filter = null, int page = 1, int pageSize = 10, string select = null)
   {
       var result = await _service.GetAllAsync<ItemDTO>(search, filter, page, pageSize, select);
       return Ok(result);
   }
   ```

---

## 4. **Entity Example**

### Item Entity
```csharp
public class Item : BaseEntity<Guid>
{
    public string Name { get; set; }
}
```

### Item DTO
```csharp
public class ItemDTO
{
    public string Name { get; set; }
}
```

### Item Request Model
```csharp
public class ItemRequest
{
    public string Name { get; set; }
}
```

---

## Summary
By inheriting and overriding the provided `MongoRepository`, `MongoBaseService`, and `MongoBaseController` classes, you can quickly create a robust and scalable application architecture. Follow the examples above to implement specific logic for your entities.

