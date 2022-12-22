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

        public bool CanMakeReservation(int restaurantId, DateTime date, int seats) {
            try {
                if (restaurantId <= 0) { throw new ReservationServiceException($"{nameof(CanMakeReservation)} - Invalid restaurantId"); }
                if (date < DateTime.Now) { throw new ReservationServiceException($"{nameof(CanMakeReservation)} - Reservations must be in the future"); }
                if (seats <= 0) { throw new ReservationServiceException($"{nameof(CanMakeReservation)} - Seats must be positive"); }

                TimeSpan halfHourEarlier = date.AddMinutes(-30).TimeOfDay;
                TimeSpan oneHourEarlier = date.AddHours(-1).TimeOfDay;

                Dictionary<int, int> tablesOfRestaurant = _restaurantRepository.GetTablesOfRestaurant(restaurantId);
                List<Reservation> reservationsOnDate = _reservationRepository.GetReservationsOnDate(restaurantId, date);

                // checking every table if there are reservations, if there is a spots and amount of seats allows us to make a reservation, return true;
                foreach (int table in tablesOfRestaurant.Keys) {
                    // Are t
                    var r = reservationsOnDate.GroupBy(r => r.Table);//.ToDictionary<Table, List<Reservation>>()
                        //&& (r.Date.TimeOfDay == date.TimeOfDay || r.Date.TimeOfDay == halfHourEarlier || r.Date.TimeOfDay == oneHourEarlier))) {
                        
                        return false;
                    
                }
                return true;

            } catch (ReservationServiceException) {
                throw;
            } catch (Exception ex) {
                throw new ReservationServiceException(nameof(CanMakeReservation), ex);
            }
        }
    }
}