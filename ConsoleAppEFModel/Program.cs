// See https://aka.ms/new-console-template for more information
using RESTaurantBL.Model;
using RESTaurantBL.Services;
using RESTaurantDLEF;
using RESTaurantDLEF.Repositories;

Console.WriteLine("Hello, World!");


string connectionstring = "Data Source=LAPTOP-BFPIKR71\\SQLEXPRESS;Initial Catalog=RESTaurant;Integrated Security=True; TrustServerCertificate=True";
RestaurantContext db = new RestaurantContext(connectionstring);
RestaurantService restaurantService = new RestaurantService(new RestaurantRepository(connectionstring));
List<Restaurant> restaurants = restaurantService.GetRestaurants();
db.Database.EnsureDeleted();
db.Database.EnsureCreated();
foreach (Restaurant r in restaurants) {
    restaurantService.AddRestaurant(r);
}
Console.WriteLine(null == null);