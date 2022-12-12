using RESTaurantBL.Exceptions;
using RESTaurantBL.Interfaces;
using RESTaurantBL.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantBL.Services {
    public class RestaurantService {
        private IRestaurantRepository restaurantRepo;

        public RestaurantService(IRestaurantRepository restaurantRepo) {
            this.restaurantRepo = restaurantRepo;
        }

        public Restaurant AddRestaurant(Restaurant restaurant) {
            try {
                if (restaurant == null) { throw new RestaurantServiceException("AddRestaurant - Restaurant is null"); }
                if (restaurantRepo.DoesExist(restaurant)) { throw new RestaurantServiceException("AddRestaurant - Restaurant already exists"); }
                restaurantRepo.AddRestaurant(restaurant);
                return restaurant;
            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException("AddRestaurant", ex);
            }
        }

        public static List<string> GetKitchenTypes() {
            return new List<string>(ConfigurationManager.AppSettings["kitchenTypes"].Split(';'));
        }

        public List<Restaurant> GetRestaurants() {
            try {
                return restaurantRepo.GetRestaurants();
            } catch (Exception ex) {
                throw new RestaurantServiceException("GetRestaurants", ex);
            }
        }

        public Restaurant GetRestaurant(int restaurantId) {
            try {
                if (!restaurantRepo.DoesExist(restaurantId)) { throw new RestaurantServiceException("GetRestaurant - RestaurantId doesn't exist"); }
                return restaurantRepo.GetRestaurant(restaurantId);
            } catch (Exception ex) {
                throw new RestaurantServiceException("GetRestaurant", ex);
            }
        }

        public bool DoesExist(int restaurantId) {
            try {
                return restaurantRepo.DoesExist(restaurantId);
            } catch (Exception ex) {
                throw new RestaurantServiceException("DoesExist", ex);
            }
        }

        public Restaurant UpdateRestaurant(Restaurant restaurant) {
            try {
                if (restaurant == null) { throw new RestaurantServiceException("UpdateRestaurant - Restaurant is null"); }
                if (!restaurantRepo.DoesExist(restaurant.RestaurantId)) { throw new RestaurantServiceException("UpdateRestaurant - Restaurant does not exist"); }
                Restaurant restaurantDB = restaurantRepo.GetRestaurant(restaurant.RestaurantId);
                if (restaurantDB.HasTheSameProperties(restaurant)) { throw new RestaurantServiceException("Restaurant hasn't changed"); }
                return restaurantRepo.UpdateRestaurant(restaurant);
            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException("UpdateRestaurant", ex);
            }
        }

        public void DeleteRestaurant(int restaurantId) {
            try {
                if (!restaurantRepo.DoesExist(restaurantId)) { throw new RestaurantServiceException("UpdateRestaurant - Restaurant does not exist"); }
                restaurantRepo.DeleteRestaurant(restaurantId);
            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException("UpdateRestaurant", ex);
            }
        }
    }
}
