using Microsoft.AspNetCore.Hosting;
using RESTaurantBL.Interfaces;
using RESTaurantBL.Services;
using RESTaurantDLEF;
using RESTaurantDLEF.Repositories;

var builder = WebApplication.CreateBuilder(args);
string connectionstring = "Data Source=LAPTOP-BFPIKR71\\SQLEXPRESS;Initial Catalog=RESTaurant;Integrated Security=True; TrustServerCertificate=True";

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IRestaurantRepository>(r => new RestaurantRepository(connectionstring));
builder.Services.AddSingleton<ICustomerRepository>(c => new CustomerRepository(connectionstring));
builder.Services.AddSingleton<IReservationRepository>(r => new ReservationRepository(connectionstring));
builder.Services.AddSingleton<RestaurantService>();
builder.Services.AddSingleton<CustomerService>();
builder.Services.AddSingleton<ReservationService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment()) {
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
