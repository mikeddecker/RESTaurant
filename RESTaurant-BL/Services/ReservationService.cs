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

        public void CancelReservation(int reservationId) {
            try {
                if (reservationId <= 0) { throw new ReservationServiceException($"{nameof(CancelReservation)} - Invalid reservationIdea"); }
                if (!_reservationRepository.DoesReservationExist(reservationId)) { throw new ReservationServiceException($"{nameof(AddReservation)} - Reservation does not exists)"); }

                _reservationRepository.CancelReservation(reservationId);
            } catch (ReservationServiceException) {
                throw;
            } catch (Exception ex) {
                throw new ReservationServiceException(nameof(CancelReservation), ex);
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

        public bool DoesReservationExist(int reservationId) {
            try {
                if (reservationId <= 0) { throw new ReservationServiceException($"{nameof(DoesReservationExist)} - Invalid reservationIdea"); }
                return _reservationRepository.DoesReservationExist(reservationId);
            } catch (ReservationServiceException) {
                throw;
            } catch (Exception ex) {
                throw new ReservationServiceException(nameof(DoesReservationExist), ex);
            }
        }

        public List<Reservation> GetReservationsOfCustomer(int customerId, DateTime beginDate, DateTime endDate) {
            try {
                if (customerId <= 0) { throw new ReservationServiceException($"{nameof(GetReservationsOfCustomer)} - Invalid customerId"); }
                if (beginDate.GetHashCode() == 0) { throw new ReservationServiceException($"{nameof(GetReservationsOfCustomer)} - beginDate hashcode 0"); }
                if (endDate.GetHashCode() == 0) { throw new ReservationServiceException($"{nameof(GetReservationsOfCustomer)} - endDate hashcode 0"); }
                if (beginDate >= endDate) { throw new ReservationServiceException($"{nameof(GetReservationsOfCustomer)} - enddate must be later than startdate"); }
                return _reservationRepository.GetReservationsOfCustomer(customerId, beginDate, endDate);

            } catch (ReservationServiceException) {
                throw;
            } catch (Exception ex) {
                throw new ReservationServiceException(nameof(GetReservationsOfCustomer), ex);
            }
        }

        private Table CheckForSmallerTable(Reservation reservation, int seats) {
            // Returns the same table if there are no changes
            try {
                (bool, int) canReserveTable = CanMakeReservation_GetTablenumber(reservation.Restaurant.RestaurantId, reservation.Date, seats);
                if (canReserveTable.Item1) {
                    // Another table available?
                    Table table = _restaurantRepository.GetTableOfRestaurant(reservation.Restaurant.RestaurantId, canReserveTable.Item2);
                    return table.Seats < reservation.Table.Seats ? table : reservation.Table;
                } else {
                    // No other table available
                    return reservation.Table;
                }

            } catch (ReservationServiceException) {
                throw;
            } catch (Exception ex) {
                throw new ReservationServiceException(nameof(CheckForSmallerTable), ex);
            }
        }

        public Reservation UpdateReservation(int reservationId, DateTime? date, int? seats) {
            try {
                if (!seats.HasValue && !date.HasValue) { throw new ReservationServiceException($"{nameof(UpdateReservation)} - Bothe date and seats are null"); }
                if (reservationId <= 0) { throw new ReservationServiceException($"{nameof(UpdateReservation)} - Invalid reservationId"); }
                if (date.HasValue && date.Value.GetHashCode() == 0) { throw new ReservationServiceException($"{nameof(UpdateReservation)} - date hashcode 0"); }
                if (date.HasValue && date.Value < DateTime.Now) { throw new ReservationServiceException($"{nameof(UpdateReservation)} - date must be in the future"); }
                if (seats.HasValue && seats.Value <= 0) { throw new ReservationServiceException($"{nameof(UpdateReservation)} - seats must be more than 0"); }

                // checking if there are changes
                bool isGonnaChange = false;
                Reservation reservationDB = _reservationRepository.GetReservation(reservationId);
                if (date.HasValue && reservationDB.Date != date.Value) { isGonnaChange = true; }
                if (seats.HasValue && reservationDB.Seats != seats.Value) { isGonnaChange = true; }
                if (!isGonnaChange) {
                    throw new ReservationServiceException($"{nameof(UpdateReservation)} - No update");
                } else {
                    // First scenario: only seats changed
                    if (!date.HasValue) {
                        // That means, seats have changed
                        // if date hasn't changed and more seats asked than initial and the reserved table still has seats left, just update the seats amount of reservation
                        if (seats.Value > reservationDB.Seats && reservationDB.Table.Seats >= seats.Value) {
                            // More customers, but still at the same table
                            return _reservationRepository.UpdateReservation_OtherCustomerAmountStillAtTheSameTable(reservationId, seats.Value);
                        } else {
                            // Less people at the table, date is the same
                            Table table = CheckForSmallerTable(reservationDB, seats.Value); // always returns a table.
                            if (table.Seats < reservationDB.Table.Seats) {
                                return _reservationRepository.UpdateReservation(reservationId, date, seats, table.TableNumber);
                            } else {
                                // less customers, but still the same table
                                return _reservationRepository.UpdateReservation_OtherCustomerAmountStillAtTheSameTable(reservationId, seats.Value);
                            }
                        }
                    } else {
                        // Date changed, so let's also just get another table
                        DateTime canMakeReservationDate = date.HasValue ? date.Value : reservationDB.Date;
                        int canMakeReservationSeats = seats.HasValue ? seats.Value : reservationDB.Seats;
                        (bool, int) canReserveTable = CanMakeReservation_GetTablenumber(reservationDB.ReservationId, canMakeReservationDate, canMakeReservationSeats);

                        if (canReserveTable.Item1) {
                            return _reservationRepository.UpdateReservation(reservationId, date, seats, canReserveTable.Item2);
                        } else {
                            throw new ReservationServiceException($"{nameof(UpdateReservation)} - Can't find a table for {canMakeReservationSeats} on {canMakeReservationDate} at {reservationDB.Restaurant.Name}");
                        }
                    }
                }
                //return _reservationRepository.UpdateReservation(reservationId, date, seats);
            } catch (ReservationServiceException) {
                throw;
            } catch (Exception ex) {
                throw new ReservationServiceException(nameof(UpdateReservation), ex);
            }
        }
    }
}