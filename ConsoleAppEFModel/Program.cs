// See https://aka.ms/new-console-template for more information
using RESTaurant_DL;

Console.WriteLine("Hello, World!");


string connectionstring = "Data Source=LAPTOP-BFPIKR71\\SQLEXPRESS;Initial Catalog=RESTaurant;Integrated Security=True; TrustServerCertificate=True";
RestaurantContext db = new RestaurantContext(connectionstring);
db.Database.EnsureDeleted();
db.Database.EnsureCreated();
Console.WriteLine(null == null);