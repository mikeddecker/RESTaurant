using RESTaurantBL.Interfaces;
using RESTaurantBL.Model;
using RESTaurantBL.Services;
using RESTaurantDLEF.Repositories;

Console.WriteLine("Hello, World!");

string connectionstring = "Data Source=LAPTOP-BFPIKR71\\SQLEXPRESS;Initial Catalog=RESTaurant;Integrated Security=True; TrustServerCertificate=True";
IRestaurantRepository restaurantRepo = new RestaurantRepository(connectionstring);
IReservationRepository reservationRepository = new ReservationRepository(connectionstring);
RestaurantService restaurantService = new RestaurantService(restaurantRepo);
ReservationService reservationService = new ReservationService(reservationRepository, restaurantRepo);

//foreach (string kitchenType in RestaurantService.GetKitchenTypes())
//{
//    Console.WriteLine(kitchenType);
//}

////restaurantRepo.AddLocation
//Restaurant cartoon = new Restaurant("Cartoon", new Location(1945, "Lebbeke"), "chinees", "info@cartoon.be", "+32478090859");
//Restaurant ratatouille = new Restaurant("Ratatouille", new Location(7500, "Paris"), "french", "info@ratatouille.be", "+32478090859");
//try
//{
//    Console.WriteLine(restaurantService.AddRestaurant(ratatouille).RestaurantId);
//}
//catch (Exception ex)
//{
//    Console.WriteLine(ex.Message);
//}


reservationService.CanMakeReservation(1, new DateTime(2022, 12, 30, 20, 30, 0), 3);

Console.WriteLine(true);