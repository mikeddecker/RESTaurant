namespace RESTaurant.Model.Input {
    public class CustomerRESTinputDTO {

        public CustomerRESTinputDTO(string name, string email, string phone, LocationRESTinputDTO location) {
            Name = name;
            Email = email;
            Phone = phone;
            Location = location;
        }

        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public LocationRESTinputDTO Location { get; set; }

        public override string ToString() {
            return $"{nameof(CustomerRESTinputDTO)}, {Name}, {Email}, {Phone}, {Location}";
        }
    }
}
