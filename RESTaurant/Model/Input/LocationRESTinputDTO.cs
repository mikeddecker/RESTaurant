namespace RESTaurant.Model.Input {
    public class LocationRESTinputDTO {
        public LocationRESTinputDTO(int postalCode, string city, string street, string housenumberlabel) {
            PostalCode = postalCode;
            City = city;
            if (street != null || street.ToLower() != "string") { Street = street; }
            if (housenumberlabel != null || housenumberlabel.ToLower() != "string") { Housenumberlabel = housenumberlabel; }
        }

        public int PostalCode { get; set; }
        public string City { get; set; }
        public string? Street { get; set; }
        public string? Housenumberlabel { get; set; }
    }
}
