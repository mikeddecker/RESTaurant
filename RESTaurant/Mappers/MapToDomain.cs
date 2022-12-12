using Microsoft.EntityFrameworkCore;
using RESTaurant.Exceptions;
using RESTaurant.Model.Input;
using RESTaurant_BL.Model;

namespace RESTaurant.Mappers {
    public class MapToDomain {
        internal static Restaurant MapRestaurant(RestaurantRESTinputDTO restaurantRESTinput) {
            try {
                Location location = MapLocation(restaurantRESTinput.Location);
                return new Restaurant(restaurantRESTinput.Name, location, restaurantRESTinput.Kitchen.ToLower(), restaurantRESTinput.Email, restaurantRESTinput.Phone);
            } catch (Exception ex) {
                throw new MapException("MapRestaurant", ex);
            }
        }

        internal static Restaurant MapRestaurant(int restaurantId, RestaurantRESTinputDTO restaurantRESTinput) {
            try {
                Restaurant r = MapRestaurant(restaurantRESTinput);
                r.SetRestaurantId(restaurantId);
                return r;
            } catch (MapException) {
                throw;
            } catch (Exception ex) {
                throw new MapException("MapRestaurant", ex);
            }
        }

        private static Location MapLocation(LocationRESTinputDTO location) {
            try {
                Location mappedLocation = new Location(location.PostalCode, location.City);
                if (!string.IsNullOrWhiteSpace(location.Street) && location.Street.ToLower() != "string") { mappedLocation.SetStreet(location.Street); }
                if (!string.IsNullOrWhiteSpace(location.Housenumberlabel) && location.Housenumberlabel.ToLower() != "string") { mappedLocation.SetHousenumber(location.Housenumberlabel); }
                return mappedLocation;
            } catch (Exception ex) {
                throw new MapException("MapLocation", ex);
            }
        }
    }
}
