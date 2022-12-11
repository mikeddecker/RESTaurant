using RESTaurant.Exceptions;
using RESTaurant.Model.Output;
using RESTaurant_BL.Model;

namespace RESTaurant.Mappers {
    public class MapToREST {
        internal static List<RestaurantRESToutputDTO> MapToListFromDomain(string hostURL, List<Restaurant> restaurants) {
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
    }
}
