// See https://aka.ms/new-console-template for more information
using RESTaurantBL.Model;

Console.WriteLine("Hello, World!");

// Restaurants
Restaurant cartoon = new Restaurant("Cartoon", new Location(1945, "Lebbele"), "french", "info@cartoon.be", "052 34 10 13");
Restaurant degroeneWandeling = new Restaurant("De groene wandeling", new Location(9255, "Buggenhout"), "french", "info@degroenewandeling.be", "052 34 54 04");
cartoon.SetRestaurantId(1);
degroeneWandeling.SetRestaurantId(2);

// Customers 
Customer mike = new Customer("Mike", "m@dd.be", "+32478090859", new Location(9255, "Buggenhout"));
mike.SetCustomerId(1);

// Reservations
Reservation reservation1 = new Reservation(1, cartoon, mike);
Reservation reservation2 = new Reservation(2, degroeneWandeling, mike);

Console.WriteLine(cartoon.Reservations);
Console.WriteLine(mike.Reservations);