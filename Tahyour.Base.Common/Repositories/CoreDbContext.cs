namespace Tahyour.Base.Common.Repositories
{
    public class CoreDbContext : ApplicationDbContext
    {
        public CoreDbContext(DbContextOptions<CoreDbContext> options) : base(options) { }
    }
}
