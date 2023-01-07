using RESTaurant.Model.Output;
using RESTaurantBL.Model;

namespace RESTaurant.Model.Input {
    public class ReservationRESTinputDTO {
        public ReservationRESTinputDTO(int restaurantId, int customerId, int seats, DateTime date) {
            RestaurantId = restaurantId;
            CustomerId = customerId;
            Seats = seats;
            Date = date;
        }

        public int RestaurantId { get; set; }
        public int CustomerId { get; set; }
        public int Seats { get; set; }
        public DateTime Date { get; set;}

        public override string ToString() {
            return $"{nameof(ReservationRESTinputDTO)}, {RestaurantId}, {CustomerId}, {Seats}, {Date.ToString()}";
        }
    }
}
