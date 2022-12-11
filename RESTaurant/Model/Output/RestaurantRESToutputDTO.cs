namespace RESTaurant.Model.Output {
    public class RestaurantRESToutputDTO {
        public RestaurantRESToutputDTO(string id, string name, LocationRESToutputDTO location, string kitchen, string email, string phone) {
            Id = id;
            Name = name;
            Location = location;
            Kitchen = kitchen;
            Email = email;
            Phone = phone;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Kitchen { get; set; }
        public LocationRESToutputDTO Location { get; set; }
    }
}
