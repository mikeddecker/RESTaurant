using Microsoft.EntityFrameworkCore;
using RESTaurant_DL.EFModel;

namespace RESTaurant_DL {
    public class RestaurantContext : DbContext {
        private string _connectionString;

        public RestaurantContext(string connectionString) {
            _connectionString = connectionString;
        }

        public DbSet<RestaurantEF> Restaurant { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            optionsBuilder.UseSqlServer(_connectionString);
        }

    }
}