using Microsoft.EntityFrameworkCore;
using RESTaurantDLEF.EFModel;
using RESTaurantDLEF.EFModel;

namespace RESTaurantDLEF {
    public class RestaurantContext : DbContext {
        private string _connectionString;

        // Constructor without a parameter because the migration needs it.
        public RestaurantContext() {
            _connectionString = "Data Source=LAPTOP-BFPIKR71\\SQLEXPRESS;Initial Catalog=RESTaurant;Integrated Security=True; TrustServerCertificate=True";
        }

        public RestaurantContext(string connectionString) {
            _connectionString = connectionString;
        }

        public DbSet<RestaurantEF> Restaurant { get; set; }
        public DbSet<TableEF> Table { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<RestaurantEF>().Property(r => r.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<TableEF>().Property(t => t.IsDeleted).HasDefaultValue(false);
        }
    }
}