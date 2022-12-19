using RESTaurantBL.Services;
using RESTaurantBL.Exceptions;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantBL.Model {
    public class Restaurant {
        private string name;
        private int restaurantId;
        private Location location;
        private string kitchen;
        private string email;
        private string phone;
        private List<Reservation> reservations;



#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        internal Restaurant(int restaurantId, string name, Location location, string kitchen, string email, string phone) {
            // No checks, because it's the constructor for the repo. Only for location, to check if its not null
            RestaurantId = restaurantId;
            Name = name;
            Location = location ?? throw new RestaurantException("Restaurant - Location is null");
            Kitchen = kitchen;
            Email = email;
            Phone = phone;
        }

        public Restaurant(string name, Location location, string kitchen, string email, string phone) {
            SetName(name);
            SetLocation(location);
            SetKitchen(kitchen);
            SetEmail(email);
            SetPhone(phone);
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int RestaurantId { get => restaurantId; private set => SetRestaurantId(value); }
        public string Name { get => name; private set => SetName(value); }
        public Location Location { get => location; private set => SetLocation(value); }
        public string Kitchen { get => kitchen; private set => SetKitchen(value); }
        public string Email { get => email; private set => SetEmail(value); }
        public string Phone { get => phone; private set => SetPhone(value); }

        public Dictionary<int, int> Tables { get; set; } // Tablenumber - Seats
        public List<Reservation> Reservations { get => reservations; private set => reservations = value; }

        public void SetRestaurantId(int id) {
            if (id <= 0) { throw new RestaurantException("SetRestaurantId - Id smaller than 1"); }
            restaurantId = id;
        }
        public void SetName(string name) {
            if (string.IsNullOrWhiteSpace(name)) { throw new RestaurantException("SetName - No name filled in"); }
            this.name = name.Trim();
        }
        public void SetLocation(Location location) {
            if (location == null) { throw new RestaurantException("SetLocation - Location is null"); }
            this.location = location;
        }
        public void SetKitchen(string kitchentype) {
            if (string.IsNullOrWhiteSpace(kitchentype)) { throw new RestaurantException("SetKitchen - No kitchentype filled in"); }
            kitchentype = kitchentype.Trim();
            if (!RestaurantService.GetKitchenTypes().Contains(kitchentype)) { throw new RestaurantException("SetKitchen - Kitchentype not recognized"); }
            kitchen = kitchentype;
        }
        public void SetEmail(string email) {
            if (string.IsNullOrWhiteSpace(email)) { throw new RestaurantException("SetEmail - No email filled in"); }
            email = email.ToLower().Trim();
            if (!Verify.IsValidEmailSyntax(email)) { throw new RestaurantException("SetEmail - Invalid email"); }
            this.email = email;

        }
        public void SetPhone(string phone) {
            if (string.IsNullOrWhiteSpace(phone)) { throw new RestaurantException("SetPhone - No phone filled in"); }
            phone = phone.Trim();
            if (!Verify.IsValidPhoneNumberBE(phone)) { throw new RestaurantException("SetPhone - Phone is not a valid BE number"); }
            this.phone = phone;
        }

        public bool HasTheSameProperties(Restaurant restaurant) {
            return RestaurantId == restaurant.RestaurantId &&
                   Name == restaurant.Name &&
                   Location.Equals(restaurant.Location) &&
                   Kitchen == restaurant.Kitchen &&
                   Email == restaurant.Email &&
                   Phone == restaurant.Phone;
        }

        public override int GetHashCode() {
            return HashCode.Combine(RestaurantId, Name, Location, Kitchen, Email, Phone);
        }

        public override bool Equals(object? obj) {
            return obj is Restaurant restaurant &&
                   restaurantId == restaurant.restaurantId;
        }
    }
}
