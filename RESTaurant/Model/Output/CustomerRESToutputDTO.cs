namespace RESTaurant.Model.Output {
    public class CustomerRESToutputDTO {
        public CustomerRESToutputDTO(string customerID, string name, string email, string phone, LocationRESToutputDTO location) {
            CustomerID = customerID;
            Name = name;
            Email = email;
            Phone = phone;
            Location = location;
        }

        public string CustomerID { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public LocationRESToutputDTO Location { get; set; }
    }
}
