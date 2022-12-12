using Microsoft.EntityFrameworkCore;
using RESTaurantDLEF.EFModel;

namespace RESTaurantDLEF {
    public class RestaurantContext : DbContext {
        private string _connectionString;

        public RestaurantContext(string connectionString) {
            _connectionString = connectionString;
        }

        public DbSet<RestaurantEF> Restaurant { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<RestaurantEF>().Property(r => r.IsDeleted).HasDefaultValue(false);
        }
    }
}