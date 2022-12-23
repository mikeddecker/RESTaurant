using RESTaurantBL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantBL.Interfaces {
    public interface IReservationRepository {
        Reservation AddReservation(Reservation reservation);
        void CancelReservation(int reservationId);
        bool DoesReservationExist(Reservation reservation);
        bool DoesReservationExist(int reservationId);
        bool DoesReservationOverlapCustomer(Reservation reservation);
        bool DoesReservationOverlapTable(Reservation reservation);
        List<Restaurant> GetAvailableRestaurants(DateTime date);
        Reservation GetReservation(int reservationId);
        List<Reservation> GetReservationsOfCustomer(int customerId, DateTime beginDate, DateTime endDate);
        Dictionary<int, Reservation> GetReservationsOnDate_Table_Reservation(int restaurantId, DateTime date);
        Reservation UpdateReservation(int reservationId, DateTime? date, int? seats, int tableNr);
        Reservation UpdateReservation_OtherCustomerAmountStillAtTheSameTable(int reservationId, int value);
    }
}
