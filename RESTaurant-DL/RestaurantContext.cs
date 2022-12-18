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
        public DbSet<CustomerEF> Customer { get; set; }
        public DbSet<LocationEF> Location { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<RestaurantEF>().Property(r => r.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<LocationEF>().Property(l => l.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<TableEF>().Property(t => t.IsDeleted).HasDefaultValue(false);
            modelBuilder.Entity<CustomerEF>().Property(c => c.IsDeleted).HasDefaultValue(false);
            //modelBuilder.Entity<LocationEF>().HasData(new LocationEF[] {new (1, 1945, "Lebbeke", null, null) });
            //modelBuilder.Entity<RestaurantEF>().Property(r => r.Location).HasConversion(l => l.LocationId, l => new LocationEF(l, 1234, "MIGRATION", null, null))
            //modelBuilder.Entity<RestaurantEF>().Property(r => r.Location).HasDefaultValue(new LocationEF(1,1234, "MIGRATION", null, null));
            //modelBuilder.Entity<CustomerEF>().Property(r => r.Location).HasDefaultValue();
        }
    }
}