namespace Tahyour.Base.Common.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddBaseServices(this IServiceCollection services)
    {
        services.AddMongo()
            .AddMongoService<AuditLog>("AuditLog");

        return services;
    }
}
