namespace RESTaurant.Model.Output {
    public class ReservationRESToutputDTO {
        public ReservationRESToutputDTO(string reservationID, DateTime reservationTime, RestaurantRESToutputDTO restaurant, int tablenumber, int seats, CustomerRESToutputDTO customer) {
            ReservationId = reservationID;
            ReservationTime = reservationTime;
            Restaurant = restaurant;
            Tablenumber = tablenumber;
            Seats = seats;
            Customer = customer;
        }

        public string ReservationId { get; set; }
        public DateTime ReservationTime { get; set; }
        public RestaurantRESToutputDTO Restaurant { get; set; }
        public int Tablenumber { get; set; }
        public int Seats { get; set; }
        public CustomerRESToutputDTO Customer { get; set; }
    }
}
