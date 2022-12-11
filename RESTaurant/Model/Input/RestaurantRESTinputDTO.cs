using RESTaurant.Model.Output;

namespace RESTaurant.Model.Input {
    public class RestaurantRESTinputDTO {
        public RestaurantRESTinputDTO(string name, string email, string phone, string kitchen, LocationRESTinputDTO location) {
            Name = name;
            Email = email;
            Phone = phone;
            Kitchen = kitchen;
            Location = location;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Kitchen { get; set; }
        public LocationRESTinputDTO Location { get; set; }
    }
}
