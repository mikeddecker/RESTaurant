using RESTaurant.Model.Output;

namespace RESTaurant.Model.Input {
    public class RestaurantRESTinputDTO {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Kitchen { get; set; }
        public LocationRESTinputDTO Location { get; set; }
    }
}
