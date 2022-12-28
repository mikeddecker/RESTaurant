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
        private DateTime _date;
        private Table _table;
        private bool _isCanceled = false;

        public Reservation(Restaurant restaurant, Customer customer, Table table, int seats, DateTime date) {
            SetDate(date);
            SetTable(table);
            SetSeats(seats);
            SetRestaurant(restaurant);
            SetCustomer(customer);
        }

        public Reservation(int reservationId, Restaurant restaurant, Customer customer, Table table, int seats, DateTime date, bool isCanceled) {
            // We do not use : this (...) because order of setting is necesairy --> id before date
            SetReservationId(reservationId);
            SetDate(date);
            SetTable(table);
            SetSeats(seats);
            SetRestaurant(restaurant);
            SetCustomer(customer);
            if (isCanceled) { SetIsCanceled(isCanceled); }
        }

        public int ReservationId { get => _reservationId; set => SetReservationId(value); }
        public Restaurant Restaurant { get => _restaurant; set => SetRestaurant(value); }
        public Customer Customer { get => _customer; set => SetCustomer(value); }
        public Table Table { get => _table; set => SetTable(value); }
        public int Seats { get => _seats; set => SetSeats(value); }
        public DateTime Date { get => _date; set => SetDate(value); }
        public bool IsCanceled { get => _isCanceled; private set => SetIsCanceled(value); }

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
        }

        private void SetCustomer(Customer customer) {
            if (customer == null) { throw new ReservationException($"{nameof(SetCustomer)} - Customer is null"); }
            if (customer.CustomerId == 0) { throw new ReservationException($"{nameof(SetCustomer)} - Customer has no idea"); }

            // Customer is customer, we can't change it.
            _customer = customer;
        }

        internal void SetTable(Table table) {
            _table = table;
        }

        public void SetSeats(int amount) {
            if (amount <= 0) { throw new ReservationException($"{nameof(SetSeats)} - Amount of seats must be positive"); }
            if (amount > _table.Seats) { throw new ReservationException($"{nameof(SetSeats)} - Can't reserve more seats than the table has"); }
            _seats = amount;
        }

        public void SetDate(DateTime date) {
            if (date.GetHashCode() == 0) { throw new ReservationException($"{nameof(SetDate)} - Date not initialized"); }

            // New reservations must be today or in the future, old reservations can be in the past
            if (_reservationId == 0 && DateTime.Now > date) { throw new ReservationException($"{nameof(SetDate)} - New reservations must be in the future"); }
            _date = date;
        }

        private void SetIsCanceled(bool canceled) {
            if (canceled == _isCanceled) throw new ReservationException($"{nameof(SetIsCanceled)} - IsCanceled is the same");
            _isCanceled = canceled;
        }

        public bool HasTheSameProperties(Reservation reservation) {
            return _reservationId == reservation._reservationId &&
                   _restaurant.Equals(reservation._restaurant) &&
                   _customer.Equals(reservation._customer) &&
                   _seats == reservation._seats &&
                   _date == reservation._date &&
                   _table.Equals(reservation._table) &&
                   _isCanceled == reservation._isCanceled;
        }
    }
}
