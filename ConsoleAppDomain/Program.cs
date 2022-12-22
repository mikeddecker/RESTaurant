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

// Tables
Table cartoontafel1 = new Table(1, 3);
Table cartoontafel2 = new Table(2, 3);

// Reservations
//Reservation reservation1 = new Reservation(1, cartoon, mike, cartoontafel1, 2, new DateTime(), new TimeOnly(18,30), false);
//Reservation reservation2 = new Reservation(2, cartoon, mike, cartoontafel2, 2, DateOnly.FromDateTime(DateTime.Today.AddDays(1)), new TimeOnly(18,30), false);
//Reservation reservationWithoutId1 = new Reservation(degroeneWandeling, mike, cartoontafel2, 2, DateOnly.FromDateTime(DateTime.Today.AddDays(2)), new TimeOnly(18, 30));
//Reservation reservationWithoutId2 = new Reservation(cartoon, mike, cartoontafel2, 2, DateOnly.FromDateTime(DateTime.Today.AddDays(2)), new TimeOnly(18, 30));
