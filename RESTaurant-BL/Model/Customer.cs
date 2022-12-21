using RESTaurantBL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace RESTaurantBL.Model {
    public class Customer {
        private int _customerId;
        private string _name;
        private string _email;
        private string _phoneNumber;
        private Location _location;
        private List<Reservation> _reservations = new List<Reservation>();

        internal Customer(int customerId, string name, string email, string phoneNumber, Location location) : this(name, email, phoneNumber, location) {
            // Constructor for customer out of DB
            SetCustomerId(customerId);
        }

        public Customer(string name, string email, string phoneNumber, Location location) {
            // Constructor for new customer from REST
            SetName(name);
            SetEmail(email);
            SetPhoneNumber(phoneNumber);
            SetLocation(location);
        }

        public int CustomerId { get => _customerId; private set => SetCustomerId(value); }
        public string Name { get => _name; set => SetName(value); }
        public string Email { get => _email; set => SetEmail(value); }
        public string PhoneNumber { get => _phoneNumber; set => SetPhoneNumber(value); }
        public Location Location { get => _location; set => SetLocation(value); }

        public List<Reservation> Reservations { get => _reservations; private set => _reservations = value; }

        public void SetCustomerId(int id) {
            if (id <= 0) { throw new CustomerException($"{nameof(SetCustomerId)} - Invalid customerId"); }
            _customerId = id;
        }

        public void SetName(string name) {
            if (string.IsNullOrWhiteSpace(name)) { throw new CustomerException($"{nameof(SetName)} - No name"); }
            _name = name;
        }
        private void SetEmail(string email) {
            if (string.IsNullOrWhiteSpace(email)) { throw new CustomerException($"{nameof(SetEmail)} - No email"); }
            if (!Verify.IsValidEmailSyntax(email)) { throw new CustomerException($"{nameof(SetEmail)} - No email"); }
            _email = email;
        }
        private void SetPhoneNumber(string phonenumber) {
            if (string.IsNullOrWhiteSpace(phonenumber)) { throw new CustomerException($"{nameof(SetPhoneNumber)} - No phone"); }
            if (!Verify.IsValidInternationalPhoneNumberOrBEnumber(phonenumber)) { throw new CustomerException($"{nameof(SetPhoneNumber)} - No email"); }
            _phoneNumber = phonenumber;
        }
        private void SetLocation(Location location) {
            if (location == null) { throw new RestaurantException($"{nameof(SetLocation)} - Location is null"); }
            _location = location;
        }

        public override bool Equals(object? obj) {
            return obj is Customer customer &&
                   _customerId == customer._customerId;
        }

        public override int GetHashCode() {
            return HashCode.Combine(_customerId);
        }

        internal bool HasTheSameProperties(Customer customer) {
            return CustomerId == customer.CustomerId &&
                   Name == customer.Name &&
                   Email == customer.Email &&
                   PhoneNumber == customer.PhoneNumber &&
                   Location.Equals(customer.Location);
        }

        internal void AddReservation(Reservation reservation) {
            // Internal method since we will not acces it directly, but always from Reservation
            if (reservation == null) { throw new CustomerException("Reservation is null"); }
            if (reservation.ReservationId == 0) { throw new CustomerException("Reservation has no idea"); }

            // Adding reservation
            if (_reservations.Contains(reservation)) {
                throw new CustomerException($"{nameof(AddReservation)} - Customer already contains reservation");
            } else {
                // Checking if restaurant of reservation is already filled in
                if (reservation.Customer != null) {
                    if (!reservation.Customer.Equals(this)) {
                        // reservation has another restaurant
                        throw new CustomerException($"{nameof(AddReservation)} - Customer of reservation is not the same");
                    } else {
                        _reservations.Add(reservation);
                    }
                } else {
                    // Reservation is made first, so we shouldn't come across this path
                    //_reservations.Add(reservation);
                    //reservation.SetRestaurant(this);
                }
            }
        }
    }
}
