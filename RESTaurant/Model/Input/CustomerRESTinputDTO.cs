namespace RESTaurant.Model.Input {
    public class CustomerRESTinputDTO {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public LocationRESTinputDTO Location { get; set; }

        public override string ToString() {
            return $"{nameof(CustomerRESTinputDTO)}, {Name}, {Email}, {Phone}, {Location}";
        }
    }
}
