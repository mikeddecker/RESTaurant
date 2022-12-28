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
                throw new ReservationServiceException(nameof(ArrangeTableNumberOrNull), ex);
            }
        }

        public Table? ArrangeTableNumberOrNull(int restaurantId, DateTime date, int seats) {
            try {
                if (restaurantId <= 0) { throw new ReservationServiceException($"{nameof(ArrangeTableNumberOrNull)} - Invalid restaurantId"); }
                if (date < DateTime.Now) { throw new ReservationServiceException($"{nameof(ArrangeTableNumberOrNull)} - Reservation-arrangements must be in the future"); }
                if (seats <= 0) { throw new ReservationServiceException($"{nameof(ArrangeTableNumberOrNull)} - Seats must be positive"); }
                return _reservationRepository.ArrangeBestFitTableOrNull(restaurantId, date, seats);
            } catch (ReservationServiceException) {
                throw;
            } catch (Exception ex) {
                throw new ReservationServiceException(nameof(ArrangeTableNumberOrNull), ex);
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

        public Reservation UpdateReservation(int reservationId, DateTime? date, int? seats) {
            try {
                if (!seats.HasValue && !date.HasValue) { throw new ReservationServiceException($"{nameof(UpdateReservation)} - Both date and seats are null, one of them must be filled in"); }
                if (reservationId <= 0) { throw new ReservationServiceException($"{nameof(UpdateReservation)} - Invalid reservationId"); }
                // So perhaps we are gonna change something
                Reservation reservationDB = _reservationRepository.GetReservation(reservationId);
                DateTime perhapsOtherReservationDate = date.HasValue ? date.Value : reservationDB.Date;
                int maybeMoreOrLessSeats = seats.HasValue ? seats.Value : reservationDB.Seats;

                // Valid information?
                if (reservationDB.Date < DateTime.Now && date.HasValue) { throw new ReservationServiceException($"{nameof(UpdateReservation)} - a reservation in the past can not change it's date"); }
                if (date.HasValue && date.Value.GetHashCode() == 0) { throw new ReservationServiceException($"{nameof(UpdateReservation)} - date hashcode 0"); }
                if (date.HasValue && date.Value < DateTime.Now) { throw new ReservationServiceException($"{nameof(UpdateReservation)} - date must be in the future"); }
                if (seats.HasValue && seats.Value <= 0) { throw new ReservationServiceException($"{nameof(UpdateReservation)} - seats must be more than 0"); }

                // Is there change in the reservation?
                if (reservationDB.Date == perhapsOtherReservationDate && reservationDB.Seats == maybeMoreOrLessSeats) { throw new ReservationServiceException($"{nameof(UpdateReservation)} - No update, date and amount of seats still remain the same"); }

                // Arrange table, throw exception if there aren't other tables
                Table arrangedTable = _reservationRepository.ArrangeBestFitTableOrNull(reservationDB.Restaurant.RestaurantId, perhapsOtherReservationDate, maybeMoreOrLessSeats) ?? throw new ReservationServiceException($"{nameof(UpdateReservation)} - No other table avaible");

                // We have another table
                if (arrangedTable.Seats != reservationDB.Table.Seats) {
                    // What could still go wrong?
                    // All tables for 4 people are occupied, and we want to reserve 4 seats and already have a table of 4 seats, but get a table arraged for 5, we don't want it.
                    // So if the new amount of seats don't fit at the current table && arrangedTable has more seats, we change it
                    // if the arranged table has less seats, we change it anyway
                    if (arrangedTable.Seats < reservationDB.Table.Seats) {
                        reservationDB.SetTable(arrangedTable);
                    } else if (arrangedTable.Seats > reservationDB.Table.Seats && maybeMoreOrLessSeats > reservationDB.Table.Seats) {
                        reservationDB.SetTable(arrangedTable);
                    }
                } // The same table is still appropriate if the new arranged table has the same amount of seats

                // Updating reservationDB
                reservationDB.SetDate(perhapsOtherReservationDate);
                reservationDB.SetSeats(maybeMoreOrLessSeats);
                return _reservationRepository.UpdateReservation(reservationDB);
            } catch (ReservationServiceException) {
                throw;
            } catch (Exception ex) {
                throw new ReservationServiceException(nameof(UpdateReservation), ex);
            }
        }

        public List<Reservation> GetReservations(int restaurantId, DateTime? day, DateTime? endDate) {
            try {
                if (day == null && endDate.HasValue) { throw new ReservationServiceException($"{nameof(GetReservations)} - Day must be filled in if endDate is filled in"); }
                return _reservationRepository.GetReservations(restaurantId, day, endDate);
            } catch (ReservationServiceException) {
                throw;
            } catch (Exception ex) {
                throw new ReservationServiceException(nameof(GetReservations), ex);
            }
        }
    }
}