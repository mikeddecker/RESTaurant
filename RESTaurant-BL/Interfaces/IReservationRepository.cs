using RESTaurantBL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantBL.Interfaces {
    public interface IReservationRepository {
        Reservation AddReservation(Reservation reservation);
        bool DoesReservationExist(Reservation reservation);
        bool DoesReservationOverlapCustomer(Reservation reservation);
        bool DoesReservationOverlapTable(Reservation reservation);
        List<Restaurant> GetAvailableRestaurants(DateTime date);
        List<Reservation> GetReservationsOfCustomer(int customerId, DateTime beginDate, DateTime endDate);
        Dictionary<int, Reservation> GetReservationsOnDate_Table_Reservation(int restaurantId, DateTime date);
    }
}
