using RESTaurant_BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant_DL.Repositories {
    public class RestaurantRepository : IRestaurantRepository {
        private RestaurantContext ctx;
        public RestaurantRepository(string connectionString) {
            ctx = new RestaurantContext(connectionString);
        }

    }
}
