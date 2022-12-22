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

        public ReservationService(IReservationRepository reservationRepository) {
            this._reservationRepository = reservationRepository;
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
    }
}