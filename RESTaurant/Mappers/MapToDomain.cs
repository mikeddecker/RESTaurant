using RESTaurant.Exceptions;
using RESTaurant.Model.Input;
using RESTaurantBL.Model;
using RESTaurantBL.Services;

namespace RESTaurant.Mappers {
    public class MapToDomain {
        internal static Customer MapCustomer(CustomerRESTinputDTO customerRESTinput) {
            try {
                Location location = MapLocation(customerRESTinput.Location);
                return new Customer(customerRESTinput.Name, customerRESTinput.Email, customerRESTinput.Phone, location);
            } catch (Exception ex) {
                throw new MapException($"{nameof(MapCustomer)}, {ex.Message}");
            }
        }

        internal static Customer MapCustomer(int customerId, CustomerRESTinputDTO customerRESTinput) {
            try {
                Customer c = MapCustomer(customerRESTinput);
                c.SetCustomerId(customerId);
                return c;
            } catch (MapException) {
                throw;
            } catch (Exception ex) {
                throw new MapException($"{nameof(MapCustomer)}, {ex.Message}");
            }
        }

        internal static Reservation MapReservation(ReservationRESTinputDTO reservationRESTinput, int tableNumber, CustomerService customerService, RestaurantService restaurantService) {
            try {
                // Existential checks & getting the other data
                if (reservationRESTinput == null) { throw new MapException($"{nameof(MapReservation)} - Reservation is null"); }
                if (reservationRESTinput.RestaurantId <= 0) { throw new MapException($"{nameof(MapReservation)} - Invalid RestaurantId"); }
                if (reservationRESTinput.CustomerId <= 0) { throw new MapException($"{nameof(MapReservation)} - Invalid CustomerId"); }

                // Yes i double check the date here insteady of waiting for the reservation to make the check, this because we do repo calls which could be slower.
                //if (reservationRESTinput.Date.Minute % 30 != 0 || reservationRESTinput.Date.Second % 30 != 0) { throw new MapException($"{nameof(MapReservation)} - Reservation time must be at 00 or at 30 minutes"); }
                Restaurant restaurant = restaurantService.GetRestaurant(reservationRESTinput.RestaurantId);
                Customer customer = customerService.GetCustomer(reservationRESTinput.CustomerId);
                Table table = restaurantService.GetTable(restaurant.RestaurantId, tableNumber);

                DateTime date = new DateTime(reservationRESTinput.Date.Year, reservationRESTinput.Date.Month, reservationRESTinput.Date.Day, reservationRESTinput.Date.Hour, reservationRESTinput.Date.Minute, 0);

                // Creating the reservation
               Reservation reservation = new Reservation(restaurant, customer, table, reservationRESTinput.Seats,date);
                return reservation;
            } catch (MapException) {
                throw;
            } catch (Exception ex) {
                throw new MapException($"{nameof(MapCustomer)}, {ex.Message}");
            }
        }

        internal static Restaurant MapRestaurant(RestaurantRESTinputDTO restaurantRESTinput) {
            try {
                Location location = MapLocation(restaurantRESTinput.Location);
                return new Restaurant(restaurantRESTinput.Name, location, restaurantRESTinput.Kitchen.ToLower(), restaurantRESTinput.Email, restaurantRESTinput.Phone);
            } catch (Exception ex) {
                throw new MapException($"{nameof(MapCustomer)}, {ex.Message}");
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
                throw new MapException($"{nameof(MapCustomer)}, {ex.Message}");
            }
        }

        private static Location MapLocation(LocationRESTinputDTO location) {
            try {
                Location mappedLocation = new Location(location.PostalCode, location.City);
                if (!string.IsNullOrWhiteSpace(location.Street) && location.Street.ToLower() != "string") { mappedLocation.SetStreet(location.Street); }
                if (!string.IsNullOrWhiteSpace(location.Housenumberlabel) && location.Housenumberlabel.ToLower() != "string") { mappedLocation.SetHousenumber(location.Housenumberlabel); }
                return mappedLocation;
            } catch (Exception ex) {
                throw new MapException($"{nameof(MapCustomer)}, {ex.Message}");
            }
        }
    }
}
