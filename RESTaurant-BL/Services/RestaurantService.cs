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
        private IRestaurantRepository _restaurantRepo;
        private IConfigurationWrapper _configWrapper;

        public RestaurantService(IRestaurantRepository restaurantRepo, IConfigurationWrapper configWrapper) {
            _restaurantRepo = restaurantRepo;
            _configWrapper = configWrapper;
        }

        #region KitchenTypes
        public List<string> GetKitchenTypes() {
            return _configWrapper.GetKitchenTypes();
        }

        public bool ContainsKitchenType(string kitchen) {
            try {
                return _configWrapper.ContainsKitchenType(kitchen);
            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException(nameof(ContainsKitchenType), ex);
            }
        }
        #endregion

        #region Restaurant CRUD
        public Restaurant AddRestaurant(Restaurant restaurant) {
            try {
                if (restaurant == null) { throw new RestaurantServiceException("AddRestaurant - Restaurant is null"); }
                if (!_configWrapper.ContainsKitchenType(restaurant.Kitchen)) { throw new RestaurantServiceException("AddRestaurant - Restaurant kitchentype invalid"); }
                if (_restaurantRepo.DoesRestaurantExist(restaurant)) { throw new RestaurantServiceException("AddRestaurant - Restaurant already exists"); }
                _restaurantRepo.AddRestaurant(restaurant);
                return restaurant;
            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException("AddRestaurant", ex);
            }
        }
    
        public Restaurant GetRestaurant(int restaurantId) {
            try {
                if (!_restaurantRepo.DoesRestaurantExist(restaurantId)) { throw new RestaurantServiceException("GetRestaurant - RestaurantId doesn't exist"); }
                return _restaurantRepo.GetRestaurant(restaurantId);
            } catch (Exception ex) {
                throw new RestaurantServiceException("GetRestaurant", ex);
            }
        }

        public List<Restaurant> GetRestaurants() {
            try {
                return _restaurantRepo.GetRestaurants();
            } catch (Exception ex) {
                throw new RestaurantServiceException("GetRestaurants", ex);
            }
        }

        public List<Restaurant> GetRestaurants(string? kitchen, int? postalCode) {
            try {
                // At least one parameter should be filled in
                if (!postalCode.HasValue && string.IsNullOrWhiteSpace(kitchen)) { throw new RestaurantServiceException($"{nameof(GetRestaurants)} - Both kitchentype and postalcode are not filled in"); }

                // Checking the filled in data, if filled in
                if (!string.IsNullOrWhiteSpace(kitchen) && !ContainsKitchenType(kitchen)) { throw new RestaurantServiceException($"{nameof(GetRestaurants)} - Invalid kitchentype"); }
                if (postalCode.HasValue) {
                    // check postal code when it has a value
                    if (postalCode.Value > 9999 || postalCode.Value < 1000) { throw new RestaurantServiceException($"{nameof(GetRestaurants)} - Invalid postal code {postalCode}"); }
                }

                return _restaurantRepo.GetRestaurants(kitchen, postalCode);
            } catch (Exception ex) {
                throw new RestaurantServiceException(nameof(GetRestaurants), ex);
            }
        }

        public Restaurant UpdateRestaurant(Restaurant restaurant) {
            try {
                if (restaurant == null) { throw new RestaurantServiceException("UpdateRestaurant - Restaurant is null"); }
                if (!_configWrapper.ContainsKitchenType(restaurant.Kitchen)) { throw new RestaurantServiceException("AddRestaurant - Restaurant kitchentype invalid"); }
                if (!_restaurantRepo.DoesRestaurantExist(restaurant.RestaurantId)) { throw new RestaurantServiceException("UpdateRestaurant - Restaurant does not exist"); }
                Restaurant restaurantDB = _restaurantRepo.GetRestaurant(restaurant.RestaurantId);
                if (restaurantDB.HasTheSameProperties(restaurant)) { throw new RestaurantServiceException("Restaurant hasn't changed"); }
                return _restaurantRepo.UpdateRestaurant(restaurant);
            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException("UpdateRestaurant", ex);
            }
        }
        
        public void DeleteRestaurant(int restaurantId) {
            try {
                if (!_restaurantRepo.DoesRestaurantExist(restaurantId)) { throw new RestaurantServiceException($"{System.Reflection.MethodBase.GetCurrentMethod().Name} - Restaurant does not exist"); }
                _restaurantRepo.DeleteRestaurant(restaurantId);
            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException(nameof(DeleteRestaurant), ex);
            }
        }
        #endregion

        #region Tables
        public void AddTable(int restaurantId, int tableNumber, int seats) {
            try {
                if (restaurantId <= 0) { throw new RestaurantServiceException($"{nameof(AddTable)} - Invalid restaurant idea"); }
                if (seats <= 0) { throw new RestaurantServiceException($"{nameof(AddTable)} - Seats must be more than 0"); }

                // Repo checks
                if (!_restaurantRepo.DoesRestaurantExist(restaurantId)) { throw new RestaurantServiceException($"{nameof(AddTable)} - RestaurantIdea does not exist"); }
                if (_restaurantRepo.HasRestaurantTableNumber(restaurantId, tableNumber)) { throw new RestaurantServiceException($"{nameof(AddTable)} - Restaurant already has a tablenumber {tableNumber}"); }

                _restaurantRepo.AddTableToRestaurant(restaurantId, tableNumber, seats);
            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException(nameof(AddTable), ex);
            }
        }

        public Dictionary<int, int> GetTables_TableNumber_Seats(int restaurantId) {
            try {
                if (restaurantId <= 0) { throw new RestaurantServiceException($"{System.Reflection.MethodBase.GetCurrentMethod().Name} - Invalid restaurant idea"); }
                if (!_restaurantRepo.DoesRestaurantExist(restaurantId)) { throw new RestaurantServiceException($"{System.Reflection.MethodBase.GetCurrentMethod().Name} - RestaurantId does not exist"); }
                return _restaurantRepo.GetTables(restaurantId);


            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException(System.Reflection.MethodBase.GetCurrentMethod().ToString(), ex);
            }
        }

        public Table GetTable(int restaurantId, int tablenumber) {
            try {
                if (!_restaurantRepo.DoesRestaurantExist(restaurantId)) { throw new RestaurantServiceException($"{nameof(GetTable)} - RestaurantIdea does not exist"); }
                if (!_restaurantRepo.HasRestaurantTableNumber(restaurantId, tablenumber)) { throw new RestaurantServiceException($"{nameof(GetTable)} - Restaurant with id {restaurantId} does not have tablenumber {tablenumber}"); }
                return _restaurantRepo.GetTable(restaurantId, tablenumber);
            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException(nameof(GetTable), ex);
            }
        }
        public void UpdateTable(int restaurantId, int tableNumber, int seats) {
            try {
                if (!_restaurantRepo.DoesRestaurantExist(restaurantId)) { throw new RestaurantServiceException($"{nameof(UpdateTable)} - Restaurant does not exist"); }
                if (!_restaurantRepo.HasRestaurantTableNumber(restaurantId, tableNumber)) { throw new RestaurantServiceException($"{nameof(UpdateTable)} - Table {tableNumber} does not exist in restaurant"); }
                Table table = _restaurantRepo.GetTable(restaurantId, tableNumber);
                if (table.Equals(new Table(tableNumber, seats))) { throw new RestaurantServiceException($"{nameof(UpdateTable)} - Table hasn't changed"); }
                _restaurantRepo.UpdateTable(restaurantId, tableNumber, seats);
            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException(nameof(UpdateTable), ex);
            }
        }

        public void DeleteTable(int restaurantId, int tablenumber) {
            try {
                if (!_restaurantRepo.DoesRestaurantExist(restaurantId)) { throw new RestaurantServiceException($"{nameof(DeleteTable)} - Restaurant does not exist"); }
                if (!_restaurantRepo.HasRestaurantTableNumber(restaurantId, tablenumber)) { throw new RestaurantServiceException($"{nameof(DeleteTable)} - Restaurant has no tablenumber {tablenumber}"); }
                _restaurantRepo.DeleteTable(restaurantId, tablenumber);
            } catch (RestaurantServiceException) {
                throw;
            } catch (Exception ex) {
                throw new RestaurantServiceException(nameof(DeleteTable), ex);
            }
        }

        #endregion


    }
}
