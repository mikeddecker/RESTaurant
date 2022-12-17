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
        internal static Restaurant MapRestaurant(RestaurantEF EFr) {
            try {
                Location location = new Location(EFr.PostalCode, EFr.City);
                if (!string.IsNullOrWhiteSpace(EFr.Street)) { location.SetStreet(EFr.Street); }
                if (!string.IsNullOrWhiteSpace(EFr.HousenumberLabel)) { location.SetHousenumber(EFr.HousenumberLabel); }
                Restaurant restaurant = new Restaurant(EFr.RestaurantId, EFr.Name, location, EFr.Kitchen, EFr.Email, EFr.Phone);
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
