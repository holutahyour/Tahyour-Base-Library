namespace Tahyour.Base.Common.Repositories
{
    public abstract class ApplicationDbContext : DbContext, IApplicationDbContext
    {
        protected ApplicationDbContext(DbContextOptions options) : base(options) { }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var currentTime = DateTime.Now;

            foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
            {
                if (entry.Entity == null) continue;

                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedBy = entry.Entity.CreatedBy ?? "SYSTEM";
                        entry.Entity.CreatedOn = currentTime;
                        entry.Entity.LastModifiedBy = "SYSTEM";
                        entry.Entity.LastModifiedOn = currentTime;
                        break;

                    case EntityState.Modified:
                        entry.Entity.LastModifiedBy = "SYSTEM";
                        entry.Entity.LastModifiedOn = currentTime;
                        break;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
