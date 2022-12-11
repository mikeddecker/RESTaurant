using RESTaurant_BL.Interfaces;
using RESTaurant_BL.Model;
using RESTaurant_BL.Services;
using RESTaurant_DL.Repositories;

Console.WriteLine("Hello, World!");

string connectionstring = "Data Source=LAPTOP-BFPIKR71\\SQLEXPRESS;Initial Catalog=RESTaurant;Integrated Security=True";
IRestaurantRepository restaurantRepo = new RestaurantRepository(connectionstring);
RestaurantService restaurantService = new RestaurantService(restaurantRepo);

foreach (string kitchenType in RestaurantService.GetKitchenTypes())
{
    Console.WriteLine(kitchenType);
}

//restaurantRepo.AddLocation
Restaurant cartoon = new Restaurant("Cartoon", new Location(1945, "Lebbeke"), "chinees", "info@cartoon.be", "+32478090859");
Console.WriteLine(restaurantService.AddRestaurant(cartoon).RestaurantId);
