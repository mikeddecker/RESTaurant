using RESTaurantBL.Exceptions;
using RESTaurantBL.Interfaces;
using RESTaurantBL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantBL.Services {
    public class ReservationService {
        private IReservationRepository _reservationRepository;
        private IRestaurantRepository _restaurantRepository;

        public ReservationService(IReservationRepository reservationRepository, IRestaurantRepository restaurantRepository) {
            _reservationRepository = reservationRepository;
            _restaurantRepository = restaurantRepository;
        }

        public Reservation AddReservation(Reservation reservation) {
            try {
                if (reservation == null) { throw new ReservationServiceException($"{nameof(AddReservation)} - Reservation is null"); }
                if (_reservationRepository.DoesReservationExist(reservation)) { throw new ReservationServiceException($"{nameof(AddReservation)} - Reservation already exists)"); }
                if (_reservationRepository.DoesReservationOverlapCustomer(reservation)) { throw new ReservationServiceException($"{nameof(AddReservation)} - Reservation overlaps with another reservation of the customer"); }
                if (_reservationRepository.DoesReservationOverlapTable(reservation)) { throw new ReservationServiceException($"{nameof(AddReservation)} - Reservation overlaps with another reservation for the reserved table or another reservation of the user"); }
                return _reservationRepository.AddReservation(reservation);
            } catch (ReservationServiceException) {
                throw;
            } catch (Exception ex) {
                throw new ReservationServiceException(nameof(AddReservation), ex);
            }
        }

        public List<Restaurant> CanIMakeReservation(DateTime date) {
            try {
                if (date.GetHashCode() == 0) {
                    throw new ReservationServiceException($"{nameof(CanIMakeReservation)} - Date hashcode 0");
                }
                if (date < DateTime.Now) {
                    throw new ReservationServiceException($"{nameof(CanIMakeReservation)} - Date can't be in the past");
                }

                return _reservationRepository.GetAvailableRestaurants(date);

            } catch (ReservationServiceException) {
                throw;
            } catch (Exception ex) {
                throw new ReservationServiceException(nameof(CanMakeReservation_GetTablenumber), ex);
            }
        }

        public (bool, int) CanMakeReservation_GetTablenumber(int restaurantId, DateTime date, int seats) {
            try {
                if (restaurantId <= 0) { throw new ReservationServiceException($"{nameof(CanMakeReservation_GetTablenumber)} - Invalid restaurantId"); }
                if (date < DateTime.Now) { throw new ReservationServiceException($"{nameof(CanMakeReservation_GetTablenumber)} - Reservations must be in the future"); }
                if (seats <= 0) { throw new ReservationServiceException($"{nameof(CanMakeReservation_GetTablenumber)} - Seats must be positive"); }

                TimeSpan halfHourEarlier = date.AddMinutes(-30).TimeOfDay;
                TimeSpan oneHourEarlier = date.AddHours(-1).TimeOfDay;

                Dictionary<int, int> tableSeats = _restaurantRepository.GetTablesOfRestaurant(restaurantId);
                int amountOfTables = tableSeats.Count();
                tableSeats = tableSeats.Where(t => t.Value >= seats).OrderBy(t => t.Value).ToDictionary(t => t.Key, t => t.Value);
                Dictionary<int, Reservation> reservations = _reservationRepository.GetReservationsOnDate_Table_Reservation(restaurantId, date);

                // check if all tables for that date are reserved
                if (amountOfTables == reservations.Count()) {
                    return (false, 0);
                } else {
                    // Not all tables are reserved
                    foreach (int tablenumber in tableSeats.Keys) {
                        // Does the table already contain a reservation on this hour?
                        // If not, return tablenumber
                        if (!reservations.ContainsKey(tablenumber)) {
                            return (true, tablenumber);
                        }
                    }
                }

                return (false, 0);

            } catch (ReservationServiceException) {
                throw;
            } catch (Exception ex) {
                throw new ReservationServiceException(nameof(CanMakeReservation_GetTablenumber), ex);
            }
        }

        public List<Reservation> GetReservationsOfCustomer(int customerId) {
            try {
                if (customerId <= 0) { throw new ReservationServiceException($"{nameof(GetReservationsOfCustomer)} - Invalid customerId"); }
                //if (date < DateTime.Now) { throw new ReservationServiceException($"{nameof(CanMakeReservation_GetTablenumber)} - Reservations must be in the future"); }
                //if (seats <= 0) { throw new ReservationServiceException($"{nameof(CanMakeReservation_GetTablenumber)} - Seats must be positive"); }

                //TimeSpan halfHourEarlier = date.AddMinutes(-30).TimeOfDay;
                //TimeSpan oneHourEarlier = date.AddHours(-1).TimeOfDay;

                return _reservationRepository.GetReservationsOfCustomer(customerId);

            } catch (ReservationServiceException) {
                throw;
            } catch (Exception ex) {
                throw new ReservationServiceException(nameof(GetReservationsOfCustomer), ex);
            }
        }
    }
}