using RESTaurant_BL.Interfaces;
using RESTaurant_BL.Model;
using RESTaurant_DL.EFModel;
using RESTaurant_DL.Exceptions;
using RESTaurant_DL.Mappers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant_DL.Repositories {
    public class RestaurantRepository : IRestaurantRepository {
        private RestaurantContext ctx;
        public RestaurantRepository(string connectionString) {
            ctx = new RestaurantContext(connectionString);
        }

        private void SaveAndClear() {
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
        }

        public Restaurant AddRestaurant(Restaurant restaurant) {
            try {
                RestaurantEF rEF = MapToDB.MapRestaurant(restaurant);
                ctx.Restaurant.Add(rEF);
                SaveAndClear();
                restaurant.SetRestaurantId(rEF.RestaurantId);
                return restaurant;
            } catch (Exception ex) {
                throw new RestaurantRepoException("AddRestaurant", ex);
            }
        }

        public bool DoesExist(Restaurant restaurant) {
            try {
                RestaurantEF restaurantEF = MapToDB.MapRestaurant(restaurant);
                return ctx.Restaurant.Any(r => r.Email == restaurantEF.Email && r.Name == restaurantEF.Name);
            } catch (Exception ex) {
                throw new RestaurantRepoException("DoesExist", ex);
            } finally {
                SaveAndClear();
            }
        }

        public List<Restaurant> GetRestaurants() {
            try {
                return ctx.Restaurant.Select(r => MapToDomain.MapRestaurant(r)).ToList();
            } catch (Exception ex) {
                throw new RestaurantRepoException("DoesExist", ex);
            } finally {
                SaveAndClear();
            }
        }

        public bool DoesExist(int restaurantId) {
            try {
                return ctx.Restaurant.Any(r => r.RestaurantId == restaurantId);
            } catch (Exception ex) {
                throw new RestaurantRepoException("DoesExist", ex);
            } finally { SaveAndClear(); }
        }

        public Restaurant GetRestaurant(int restaurantId) {
            try {
                return MapToDomain.MapRestaurant(ctx.Restaurant.Single(r => r.RestaurantId == restaurantId));
            } catch (Exception ex) {
                throw new RestaurantRepoException("GetRestaurant", ex);
            } finally {
                SaveAndClear();
            }
        }
    }
}
