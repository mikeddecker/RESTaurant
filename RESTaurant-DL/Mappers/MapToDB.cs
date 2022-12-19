using RESTaurantBL.Model;
using RESTaurantDLEF.EFModel;
using RESTaurantDLEF.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantDLEF.Mappers {
    internal class MapToDB {
        internal static CustomerEF MapCustomer(Customer customer) {
            try {
                CustomerEF customerEF = new CustomerEF(customer.Name, customer.Email, customer.PhoneNumber, MapLocation(customer.Location));
                return customerEF;
            } catch (Exception ex) {
                throw new MapException(nameof(MapCustomer), ex);
            }
        }

        internal static RestaurantEF MapRestaurant(Restaurant restaurant) {
            try {
                RestaurantEF restaurantEF = new RestaurantEF(restaurant.Name, restaurant.Kitchen, restaurant.Email, restaurant.Phone, MapToDB.MapLocation(restaurant.Location));
                return restaurantEF;
            } catch (Exception ex) {
                throw new MapException("MapRestaurant", ex);
            }
        }

        private static LocationEF MapLocation(Location location) {
            try {
                LocationEF locationEF = new LocationEF(location.PostalCode, location.City, location.Street, location.Housenumber);
                return locationEF;
            } catch (Exception ex) {
                throw new MapException("MapRestaurant", ex);
            }
        }
    }
}
