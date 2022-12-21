// See https://aka.ms/new-console-template for more information
using RESTaurantBL.Model;

Console.WriteLine("Hello, World!");

Restaurant cartoon = new Restaurant("Cartoon", new Location(1945, "Lebbele"), "french", "info@cartoon.be", "052 34 10 13");
cartoon.SetRestaurantId(1);

Reservation reservation1 = new Reservation(1, cartoon);
Reservation reservation2 = new Reservation(1, cartoon);
Console.WriteLine(cartoon.Reservations);