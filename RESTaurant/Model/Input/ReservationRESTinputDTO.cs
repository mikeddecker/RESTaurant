namespace RESTaurant.Model.Input {
    public class ReservationRESTinputDTO {
        public int RestaurantId { get; set; }
        public int CustomerId { get; set; }
        public int Seats { get; set; }
        public DateTime Date { get; set;}
    }
}
