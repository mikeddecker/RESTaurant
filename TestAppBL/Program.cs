// See https://aka.ms/new-console-template for more information
using RESTaurant_BL.Interfaces;
using RESTaurant_BL.Model;
using RESTaurant_BL.Services;
using RESTaurant_DL.Repositories;

Console.WriteLine("Hello, World!");

string connectionstring = "Data Source=LAPTOP-BFPIKR71\\SQLEXPRESS;Initial Catalog=RESTaurant;Integrated Security=True";
IRestaurantRepository restaurantRepo = new RestaurantRepository(connectionstring);
RestaurantService restaurantService = new RestaurantService(restaurantRepo);

foreach (string kitchenType in RestaurantService.GetKitchenTypes()) {
    Console.WriteLine(kitchenType);
}

Location locationCartoon = new Location(1945, "Lebbeke");
//restaurantRepo.AddLocation
//Restaurant cartoon = new Restaurant("Cartoon", )