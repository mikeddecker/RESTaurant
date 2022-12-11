using RESTaurant_BL.Interfaces;
using RESTaurant_BL.Model;
using RESTaurant_DL.EFModel;
using RESTaurant_DL.Mappers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant_DL.Repositories
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private RestaurantContext ctx;
        public RestaurantRepository(string connectionString)
        {
            ctx = new RestaurantContext(connectionString);
        }

        private void SaveAndClear()
        {
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
        }

        public Restaurant AddRestaurant(Restaurant restaurant)
        {
            RestaurantEF rEF = MapToDB.MapRestaurant(restaurant);
            ctx.Restaurant.Add(rEF);
            SaveAndClear();
            restaurant.SetRestaurantId(rEF.RestaurantId);
            return restaurant;
        }

        public bool DoesExist(Restaurant restaurant)
        {
            RestaurantEF restaurantEF = MapToDB.MapRestaurant(restaurant);
            return ctx.Restaurant.Any(r => r.Email == restaurantEF.Email && r.Name == restaurantEF.Name);
        }

    }
}
