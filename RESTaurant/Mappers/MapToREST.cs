using RESTaurant.Exceptions;
using RESTaurant.Model.Output;
using RESTaurantBL.Model;
using RESTaurantBL.Services;

namespace RESTaurant.Mappers {
    public class MapToREST {
        internal static List<RestaurantRESToutputDTO> MapRestaurantList(string hostURL, List<Restaurant> restaurants) {
            return restaurants.Select(r => MapRestaurant(hostURL, r)).ToList();
        }
        internal static RestaurantRESToutputDTO MapRestaurant(string hostURL, Restaurant restaurant) {
            try {
                string restaurantURL = $"{hostURL}/{restaurant.RestaurantId}";
                return new RestaurantRESToutputDTO(restaurantURL, restaurant.Name, MapLocation(restaurant.Location), restaurant.Kitchen, restaurant.Email, restaurant.Phone);
            } catch (Exception ex) {
                throw new MapException("MapRestaurant", ex);
            }
        }

        private static LocationRESToutputDTO MapLocation(Location location) {
            try {
                return new LocationRESToutputDTO(location.PostalCode, location.City, location.Street, location.Housenumber);
            } catch (Exception ex) {
                throw new MapException("MapLocation", ex);
            }
        }

        internal static RestaurantDetailRESToutputDTO MapRestaurantDetails(string hostURL, int restaurantId, RestaurantService restaurantService) {
            try {
                string restaurantURL = $"{hostURL}/{restaurantId}/Details";
                Restaurant restaurant = restaurantService.GetRestaurant(restaurantId);
                return new RestaurantDetailRESToutputDTO(restaurantURL, restaurant.Name, MapLocation(restaurant.Location), restaurant.Kitchen, restaurant.Email, restaurant.Phone, restaurantService.GetTablesOfRestaurant(restaurantId));

            } catch (Exception ex) {
                throw new MapException("MapRestaurant", ex);
            }
        }

        internal static CustomerRESToutputDTO MapCustomer(string hostURL, Customer customer) {
            try {
                string customerURL = $"{hostURL}/{customer.CustomerId}";
                return new CustomerRESToutputDTO(customerURL, customer.Name, customer.Email, customer.PhoneNumber, MapLocation(customer.Location));
            } catch (Exception ex) {
                throw new MapException(nameof(MapCustomer), ex);
            }
        }

        internal static List<CustomerRESToutputDTO> MapCustomerList(string hostURL, List<Customer> customers) {
            try {
                return customers.Select(c => MapCustomer(hostURL, c)).ToList();
            } catch (Exception ex) {
                throw new MapException($"{nameof(MapCustomerList)} - {ex.Message}");
            }
        }
    }
}
