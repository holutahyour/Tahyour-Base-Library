namespace Tahyour.Base.Common.Services
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddMongo()
                .AddMongoService<AuditLog>("AuditLog")
                .AddMongoService<Item>("Items");

            return services;
        }
    }
}
