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
        internal static Customer MapCustomer(CustomerEF cEF) {
            try {
                Customer customer = new Customer(cEF.CustomerId, cEF.Name, cEF.Email, cEF.Phone, MapLocation(cEF.Location));
                return customer;
            } catch (Exception ex) {
                throw new MapException(nameof(MapCustomer), ex);
            }
        }

        internal static Restaurant MapRestaurant(RestaurantEF restaurantEF) {
            try {
                Restaurant restaurant = new Restaurant(restaurantEF.RestaurantId, restaurantEF.Name, MapLocation(restaurantEF.Location), restaurantEF.Kitchen, restaurantEF.Email, restaurantEF.Phone);
                return restaurant;
            } catch (Exception ex) {
                throw new MapException(nameof(MapRestaurant), ex);
            }
        }

        private static Location MapLocation(LocationEF locationEF) {
            Location location = new Location(locationEF.PostalCode, locationEF.City);
            if (!string.IsNullOrWhiteSpace(locationEF.Street)) { location.SetStreet(locationEF.Street); }
            if (!string.IsNullOrWhiteSpace(locationEF.HousenumberLabel)) { location.SetHousenumber(locationEF.HousenumberLabel); }
            return location;
        }

        internal static Table MapTable(TableEF tableEFDB) {
            try {
                return new Table(tableEFDB.Tablenumber, tableEFDB.Seats);
            } catch (Exception ex) {
                throw new MapException(nameof(MapTable), ex);
            }
        }

        internal static Reservation MapReservation(ReservationEF r) {
            try {
                Restaurant restaurant = MapRestaurant(r.Restaurant);
                Customer customer = MapCustomer(r.Customer);
                Table table = MapTable(r.Table);
                return new Reservation(r.ReservationId, restaurant, customer, table, r.Seats, r.Date, r.IsCanceled);
            } catch (Exception ex) {
                throw new MapException(nameof(MapReservation), ex);
            }
        }
    }
}
