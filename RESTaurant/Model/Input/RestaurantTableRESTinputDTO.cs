namespace RESTaurant.Model.Input {
    public class RestaurantTableRESTinputDTO {
        public int TableNumber { get; set; }
        public int Seats { get; set; }
        public override string ToString() {
            return $"{nameof(RestaurantTableRESTinputDTO)}, {TableNumber}, {Seats}";
        }
    }
}
