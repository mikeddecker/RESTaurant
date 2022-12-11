using RESTaurant_BL.Exceptions;
using RESTaurant_BL.Interfaces;
using RESTaurant_BL.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant_BL.Services {
    public class RestaurantService {
        private IRestaurantRepository restaurantRepo;

        public RestaurantService(IRestaurantRepository restaurantRepo) {
            this.restaurantRepo = restaurantRepo;
        }

        public Restaurant AddRestaurant(Restaurant restaurant)
        {
            try
            {
                if (restaurant == null) { throw new RestaurantServiceException("AddRestaurant - Restaurant is null"); }
                if (restaurantRepo.DoesExist(restaurant)) { throw new RestaurantServiceException("AddRestaurant - Restaurant already exists"); }
                restaurantRepo.AddRestaurant(restaurant);
                return restaurant;
            }
            catch (RestaurantServiceException)
            {
                throw;
            }
            catch (Exception ex)
            {
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
    }
}
