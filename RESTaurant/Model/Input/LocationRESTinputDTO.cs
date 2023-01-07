namespace RESTaurant.Model.Input {
    public class LocationRESTinputDTO {
        public LocationRESTinputDTO(int postalCode, string city, string? street, string? housenumberlabel) {
            PostalCode = postalCode;
            City = city;
            Street = street;
            Housenumberlabel = housenumberlabel;
        }

        public int PostalCode { get; set; }
        public string City { get; set; }
        public string? Street { get; set; }
        public string? Housenumberlabel { get; set; }

        public override string ToString() {
            return $"{nameof(LocationRESTinputDTO)}, {PostalCode}, {City}, {Street}, {Housenumberlabel}";
        }
    }
}
