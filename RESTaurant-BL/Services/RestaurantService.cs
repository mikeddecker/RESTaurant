using RESTaurantBL.Exceptions;
using RESTaurantBL.Interfaces;
using RESTaurantBL.Model;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
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
                if (restaurantRepo.DoesRestaurantExist(restaurant)) { throw new RestaurantServiceException("AddRestaurant - Restaurant already exists"); }
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
                if (!restaurantRepo.DoesRestaurantExist(restaurantId)) { throw new RestaurantServiceException("GetRestaurant - RestaurantId doesn't exist"); }
                return restaurantRepo.GetRestaurant(restaurantId);
            } catch (Exception ex) {
                throw new RestaurantServiceException("GetRestaurant", ex);
            }
        }

        public bool DoesExist(int restaurantId) {
            try {
                return restaurantRepo.DoesRestaurantExist(restaurantId);
            } catch (Exception ex) {
                throw new RestaurantServiceException("DoesExist", ex);
            }
        }

        public Restaurant UpdateRestaurant(Restaurant restaurant) {
            try {
                if (restaurant == null) { throw new RestaurantServiceException("UpdateRestaurant - Restaurant is null"); }
                if (!restaurantRepo.DoesRestaurantExist(restaurant.RestaurantId)) { throw new RestaurantServiceException("UpdateRestaurant - Restaurant does not exist"); }
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
                if (!restaurantRepo.DoesRestaurantExist(restaurantId)) { throw new RestaurantServiceException($"{System.Reflection.MethodBase.GetCurrentMethod().Name} - Restaurant does not exist"); }
                restaurantRepo.DeleteRestaurant(restaurantId);
            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        public void AddTableToRestaurant(int restaurantId, int tableNumber, int seats) {
            try {
                if (restaurantId <= 0) { throw new RestaurantServiceException($"{System.Reflection.MethodBase.GetCurrentMethod().Name} - Invalid restaurant idea"); }
                if (seats <= 0) { throw new RestaurantServiceException($"{System.Reflection.MethodBase.GetCurrentMethod().Name} - Seats must be more than 0"); }

                // Repo checks
                if (!restaurantRepo.DoesRestaurantExist(restaurantId)) { throw new RestaurantServiceException($"{System.Reflection.MethodBase.GetCurrentMethod().Name} - RestaurantIdea does not exist"); }
                if (restaurantRepo.HasRestaurantTableNumber(restaurantId, tableNumber)) { throw new RestaurantServiceException($"{System.Reflection.MethodBase.GetCurrentMethod().Name} - Restaurant already has a tablenumber {tableNumber}"); }

                restaurantRepo.AddTableToRestaurant(restaurantId, tableNumber, seats);
            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException(System.Reflection.MethodBase.GetCurrentMethod().Name, ex);
            }
        }

        public Dictionary<int, int> GetTablesOfRestaurant(int restaurantId) {
            try {
                if (restaurantId <= 0) { throw new RestaurantServiceException($"{System.Reflection.MethodBase.GetCurrentMethod().Name} - Invalid restaurant idea"); }
                if (!restaurantRepo.DoesRestaurantExist(restaurantId)) { throw new RestaurantServiceException($"{System.Reflection.MethodBase.GetCurrentMethod().Name} - RestaurantId does not exist"); }
                return restaurantRepo.GetTablesOfRestaurant(restaurantId);


            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException(System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex);
            }
        }

        public bool HasRestaurantTableNumber(int restaurantId, int tablenumber) {
            try {
                if (!restaurantRepo.DoesRestaurantExist(restaurantId)) { throw new RestaurantServiceException($"{nameof(HasRestaurantTableNumber)} - RestaurantIdea does not exist"); }
                return restaurantRepo.HasRestaurantTableNumber(restaurantId, tablenumber);
            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException(nameof(HasRestaurantTableNumber), ex);
            }
        } // TODO : check of de method niet verwijderd mag worden

        public void DeleteTableOfRestaurant(int restaurantId, int tablenumber) {
            try {
                if (!restaurantRepo.DoesRestaurantExist(restaurantId)) { throw new RestaurantServiceException($"{nameof(DeleteTableOfRestaurant)} - Restaurant does not exist"); }
                if (!restaurantRepo.HasRestaurantTableNumber(restaurantId, tablenumber)) { throw new RestaurantServiceException($"{nameof(DeleteTableOfRestaurant)} - Restaurant has no tablenumber {tablenumber}"); }
                restaurantRepo.DeleteTableOfRestaurant(restaurantId, tablenumber);
            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException(nameof(DeleteTableOfRestaurant), ex);
            }
        }

        public void UpdateTableOfRestaurant(int restaurantId, int tableNumber, int seats) {
            try {
                if (!restaurantRepo.DoesRestaurantExist(restaurantId)) { throw new RestaurantServiceException($"{nameof(UpdateTableOfRestaurant)} - Restaurant does not exist"); }
                if (!restaurantRepo.HasRestaurantTableNumber(restaurantId, tableNumber)) { throw new RestaurantServiceException($"{nameof(UpdateTableOfRestaurant)} - Table {tableNumber} does not exist in restaurant"); }
                Table table = restaurantRepo.GetTableOfRestaurant(restaurantId, tableNumber);
                if (table.Equals(new Table(tableNumber, seats))) { throw new RestaurantServiceException($"{nameof(UpdateTableOfRestaurant)} - Table hasn't changed"); }
                restaurantRepo.UpdateTableOfRestaurant(restaurantId, tableNumber, seats);
            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException(nameof(UpdateTableOfRestaurant), ex);
            }
        }
    }
}
