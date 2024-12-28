using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Tahyour.Base.Common.Services.Implementation;

namespace Tahyour.Base.Common.Services;

public static class Extensions
{
    public static IServiceCollection AddMongo(this IServiceCollection services)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(MongoDB.Bson.BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(MongoDB.Bson.BsonType.String));


        services.AddSingleton(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            // Get the settings sections
            var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>()
                                  ?? throw new InvalidOperationException("ServiceSettings configuration is missing.");
            var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>()
                                  ?? throw new InvalidOperationException("MongoDbSettings configuration is missing.");

            // Create MongoDB client and database
            var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
            return mongoClient.GetDatabase(serviceSettings.ServiceName
                   ?? throw new InvalidOperationException("ServiceName is missing in ServiceSettings."));
        });

        return services;
    }

    public static IServiceCollection AddMongoService<T>(this IServiceCollection services, string collectionName) where T : BaseEntity<Guid>
    {
        var mapperConfig = new MapperConfiguration(mc =>
        {
            mc.AddProfile(new AutoMapperConfig());
        });

        IMapper mapper = mapperConfig.CreateMapper();

        services.AddSingleton(mapper);
        services.AddHttpContextAccessor();

        services.AddSingleton<IMongoRepository<T>>(serviceProvider =>
        {
            var database = serviceProvider.GetRequiredService<IMongoDatabase>();
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

            return new MongoRepository<T>(httpContextAccessor, database, collectionName);
        });

        services.AddSingleton<IMongoBaseService<T>>(serviceProvider =>
        {
            var database = serviceProvider.GetRequiredService<IMongoDatabase>();
            var repository = serviceProvider.GetRequiredService<IMongoRepository<T>>();
            var auditLogRepository = serviceProvider.GetRequiredService<IMongoRepository<AuditLog>>();
            var mapper = serviceProvider.GetRequiredService<IMapper>();
            var httpContextAccessor = serviceProvider.GetRequiredService<IHttpContextAccessor>();

            return new MongoBaseService<T>(repository, auditLogRepository, mapper, httpContextAccessor);
        });

        //services.AddScoped<IMongoBaseService<T>, MongoBaseService<T>>();

        return services;
    }
}
