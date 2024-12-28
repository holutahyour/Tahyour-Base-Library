# Guide to Inheriting and Overriding Repository, Service, and Presentation Layers

This document explains how to extend and customize the provided `MongoRepository`, `MongoBaseService`, and `MongoBaseController` classes in the `Tahyour.Base.Common` namespace. By inheriting and overriding these base classes, you can implement custom logic specific to your application.

---

## Table of Contents

1. [Adding the MongoDB Extension](#adding-the-mongodb-extension)
2. [Repository Layer](#repository-layer)
   - [Creating a Custom Repository](#creating-a-custom-repository)
   - [Overriding Methods](#overriding-methods)
3. [Service Layer](#service-layer)
   - [Creating a Custom Service](#creating-a-custom-service)
   - [Overriding Methods](#overriding-methods-1)
4. [Presentation Layer](#presentation-layer)
   - [Creating a Custom Controller](#creating-a-custom-controller)
   - [Overriding Methods](#overriding-methods-2)
5. [Example Entity](#example-entity)

---

## Adding the MongoDB Extension

The `MongoDB` functionality is provided as part of the `Tahyour.Base.Library` package. To add and configure it:

### Step 1: Install the Package

Run the following command to install the package:

```bash
dotnet add package Tahyour.Base.Library
```

### Step 2: Configure MongoDB Settings

Add the following sections to your `appsettings.json` file:

```json
{
  "ServiceSettings": {
    "ServiceName": "YourServiceName"
  },
  "MongoDbSettings": {
    "Host": "localhost",
    "Port": 27017
  }
}
```

### Step 3: Register the Services

In your `Startup.cs` or `Program.cs` file, use the following code to register the MongoDB services:

```csharp
builder.Services.AddMongo();
builder.Services.AddMongoService<Item>("ItemsCollection");
```

Replace `Item` and `"ItemsCollection"` with your entity type and MongoDB collection name.

### Step 4: Extend Base Classes

Follow the sections below to extend the repository, service, and controller for custom functionality.

---

## Repository Layer

### Creating a Custom Repository

To create a custom repository for your entity:

1. Create a new class that inherits from `MongoRepository<T>`.
2. Pass your entity type as the generic parameter `T`.
3. Implement additional methods or override existing ones.

```csharp
public class CustomItemRepository : MongoRepository<Item>
{
    public CustomItemRepository(IHttpContextAccessor httpContextAccessor, IMongoDatabase database)
        : base(httpContextAccessor, database, "CustomItems")
    {
    }

    public async Task<IList<Item>> GetItemsByNameAsync(string name)
    {
        var filter = Builders<Item>.Filter.Eq(i => i.Name, name);
        return await mongoCollection.Find(filter).ToListAsync();
    }
}
```

### Overriding Methods

You can override methods in the base repository to modify their behavior:

```csharp
public override async Task<Item?> GetByIdAsync(Guid id)
{
    var item = await base.GetByIdAsync(id);
    if (item == null)
    {
        throw new Exception("Item not found");
    }
    return item;
}
```

---

## Service Layer

### Creating a Custom Service

To create a custom service for your entity:

1. Create a new class that inherits from `MongoBaseService<T>`.
2. Pass your entity type as the generic parameter `T`.
3. Implement additional methods or override existing ones.

```csharp
public class CustomItemService : MongoBaseService<Item>
{
    public CustomItemService(
        IMongoRepository<Item> baseRepository,
        IMongoRepository<AuditLog> auditLogRepository,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor
    ) : base(baseRepository, auditLogRepository, mapper, httpContextAccessor)
    {
    }

    public async Task<Result<IList<ItemDTO>>> GetItemsByNameAsync(string name)
    {
        var items = await _baseRepository.GetAllAsync(i => i.Name.Contains(name));
        return new Result<IList<ItemDTO>>(true, _mapper.Map<IList<ItemDTO>>(items), "Items retrieved successfully");
    }
}
```

### Overriding Methods

You can override methods in the base service to modify their behavior:

```csharp
public override async Task<Result<bool>> RemoveAsync(Guid id)
{
    var existingItem = await _baseRepository.GetByIdAsync(id);
    if (existingItem == null)
    {
        return new Result<bool>(false, false, "Item not found");
    }
    return await base.RemoveAsync(id);
}
```

---

## Presentation Layer

### Creating a Custom Controller

To create a custom controller for your entity:

1. Create a new class that inherits from `MongoBaseController<T, TResponse>`.
2. Pass your entity type as `T` and the DTO type as `TResponse`.
3. Implement additional endpoints or override existing ones.

```csharp
[Route("api/custom-items")]
public class CustomItemController : MongoBaseController<Item, ItemDTO>
{
    public CustomItemController(IMongoBaseService<Item> service) : base(service)
    {
    }

    [HttpGet("by-name/{name}")]
    public async Task<ActionResult> GetItemsByNameAsync(string name)
    {
        var result = await _service.GetAllAsync<ItemDTO>(search: name);
        return Ok(result);
    }
}
```

### Overriding Methods

You can override methods in the base controller to modify their behavior:

```csharp
public override async Task<ActionResult> GetByIdAsync(Guid id)
{
    var response = await base.GetByIdAsync(id);
    if (response == null)
    {
        return NotFound("Custom item not found.");
    }
    return Ok(response);
}
```

---

## Example Entity

Here is an example entity and DTO used in the examples above:

```csharp
public class Item : BaseEntity<Guid>
{
    public string Name { get; set; }
}

public class ItemDTO
{
    public string Name { get; set; }
}
```

