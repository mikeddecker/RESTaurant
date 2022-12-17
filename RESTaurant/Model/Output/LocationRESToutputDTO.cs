namespace RESTaurantBL.Model.Output {
    public class LocationRESToutputDTO {
        public LocationRESToutputDTO(int postalCode, string city, string street, string housenumberlabel) {
            PostalCode = postalCode;
            City = city;
            Street = street;
            Housenumberlabel = housenumberlabel;
        }

        public int PostalCode { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string Housenumberlabel { get; set; }
    }
}
