using RESTaurantBL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantBL.Model {
    public class Reservation {
        private int _reservationId;
        private Restaurant _restaurant;
        private Customer _customer;
        private int _seats;
        private DateOnly _date;
        private TimeOnly _time;
        private Table _table;
        private bool _isCanceled;

        public Reservation(int reservationId, Restaurant restaurant, Customer customer) {
            SetReservationId(reservationId);
            SetRestaurant(restaurant);
            SetCustomer(customer);
        }

        public int ReservationId { get => _reservationId; set => SetReservationId(value); }
        public Restaurant Restaurant { get => _restaurant; set => SetRestaurant(value); }
        public Customer Customer { get => _customer; set => SetCustomer(value); }
        public Table Table { get => _table; set => SetTable(value); }
        public int Seats { get => _seats; set => SetSeats(value); }
        public DateOnly Date { get => _date; set => _date = value; }
        public TimeOnly Time { get => _time; set => _time = value; }
        public bool IsCanceled { get => _isCanceled; private set => _isCanceled = value; }

        public override bool Equals(object? obj) {
            return obj is Reservation reservation &&
                   _reservationId == reservation._reservationId;
        }

        public override int GetHashCode() {
            return HashCode.Combine(_reservationId);
        }

        internal void SetReservationId(int id) {
            if (id <= 0) { throw new ReservationException($"{nameof(SetReservationId)} - Invalid reservationId"); }
            _reservationId = id;
        }

        private void SetRestaurant(Restaurant restaurant) {
            if (restaurant == null) { throw new ReservationException($"{nameof(SetRestaurant)} - Restaurant is null"); }
            if (restaurant.RestaurantId == 0) { throw new ReservationException($"{nameof(SetRestaurant)} - Restaurant has no idea"); }

            // Restaurant can't be another, since we can only call this method privately and we use it in the constructor, so _restaurant should be null
            // If customer is wrong and chose the wrong restaurant, we cancel or remove this one and make another.
            _restaurant = restaurant;

            // Adding reservation to restaurant
            if (!restaurant.Reservations.Contains(this)) {
                restaurant.AddReservation(this);
            } else {
                // This method is called from the Restaurant, but a reservation should have a restaurant by default in the constructor 
            }
        }

        private void SetCustomer(Customer customer) {
            if (customer == null) { throw new ReservationException($"{nameof(SetCustomer)} - Customer is null"); }
            if (customer.CustomerId == 0) { throw new ReservationException($"{nameof(SetCustomer)} - Customer has no idea"); }

            // Customer is customer, we can't change it.
            _customer = customer;

            // Adding reservation to customer
            if (!customer.Reservations.Contains(this)) {
                customer.AddReservation(this);
            } else {
                // We initiat the customer from within this constructor, so we do not require an else path.
            }
        }

        internal void SetTable(Table table) {
            _table = table;
        }

        public void SetSeats(int amount) {
            if (amount <= 0) { throw new ReservationException($"{nameof(SetSeats)} - Amount of seats must be positive"); }
            if (amount > _table.Seats) { throw new ReservationException($"{nameof(SetSeats)} - Can't reserve more seats than the table has"); }
            _seats = amount;
        }

        public void SetDate(DateOnly date) {
            if (date.GetHashCode() == 0) { throw new ReservationException($"{nameof(SetDate)} - Date not initialized"); }

            // New reservations must be today or in the future
            if (_reservationId == 0 && DateOnly.FromDateTime(DateTime.Today) <= date) {
                _date = date;
            }
        }
    }
}
