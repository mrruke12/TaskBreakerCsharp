namespace TaskBreakerApi.Data {
    public class AppDbContext : DbContext {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Objective> Objectives { get; set; }
        public DbSet<OElement> OElements { get; set; }
    }
}
