using RESTaurantBL.Model;
using RESTaurantDLEF.EFModel;
using RESTaurantDLEF.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantDLEF.Mappers {
    internal class MapToDomain {
        internal static Restaurant MapRestaurant(RestaurantEF restaurantEF) {
            try {
                Location location = new Location(restaurantEF.Location.PostalCode, restaurantEF.Location.City);
                if (!string.IsNullOrWhiteSpace(restaurantEF.Location.Street)) { location.SetStreet(restaurantEF.Location.Street); }
                if (!string.IsNullOrWhiteSpace(restaurantEF.Location.HousenumberLabel)) { location.SetHousenumber(restaurantEF.Location.HousenumberLabel); }
                Restaurant restaurant = new Restaurant(restaurantEF.RestaurantId, restaurantEF.Name, location, restaurantEF.Kitchen, restaurantEF.Email, restaurantEF.Phone);
                return restaurant;
            } catch (Exception ex) {
                throw new MapException(nameof(MapRestaurant), ex);
            }
        }

        internal static Table MapTable(TableEF tableEFDB) {
            try {
                return new Table(tableEFDB.Tablenumber, tableEFDB.Seats);
            } catch (Exception ex) {
                throw new MapException(nameof(MapTable), ex);
            }
        }
    }
}
