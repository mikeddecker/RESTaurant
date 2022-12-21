using RESTaurantBL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantBL.Model
{
    public class Reservation
    {
        private int _reservationId;
        private Restaurant _restaurant;
        private Customer _customer;
        private int _seats;
        private DateOnly _date;
        private TimeOnly _time;
        private Table _table;
        private bool _isCanceled;

        public int ReservationId { get => _reservationId; set => _reservationId = value; }
        public Restaurant Restaurant { get => _restaurant; set => _restaurant = value; }
        public Customer Customer { get => _customer; set => _customer = value; }
        public int Seats { get => _seats; set => _seats = value; }
        public DateOnly Date { get => _date; set => _date = value; }
        public TimeOnly Time { get => _time; set => _time = value; }
        public Table Table { get => _table; set => _table = value; }
        public bool IsCanceled { get => _isCanceled; private set => _isCanceled = value; }

        public void SetReservationId(int id)
        {
            if (id <= 0) { throw new ReservationException($"{nameof(SetReservationId)} - Invalid reservationId"); }
            _reservationId = id;
        }

        public void SetRestaurant(Restaurant restaurant) {
            if (restaurant == null) { throw new ReservationException($"{nameof(SetRestaurant)} - Restaurant is null"); }
            if (restaurant.RestaurantId == 0) { throw new ReservationException($"{nameof(SetRestaurant)} - Restaurant has no idea");}
            
            _restaurant = restaurant;

            // Adding reservation to restaurant
            if (!restaurant.Reservations.Contains(this)) {
                
                restaurant.AddReservation(this);
            }
        }
    }
}
