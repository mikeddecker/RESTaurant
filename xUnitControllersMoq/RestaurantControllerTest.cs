using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using RESTaurant.Controllers;
using RESTaurant.Model.Input;
using RESTaurant.Model.Output;
using RESTaurantBL.Interfaces;
using RESTaurantBL.Model;
using RESTaurantBL.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xUnitControllersMoq {
    public class RestaurantControllerTest {
        private readonly Mock<IReservationRepository> _mockRepoReservation;
        private readonly Mock<IRestaurantRepository> _mockRepoRestaurant;
        private readonly Mock<IConfigurationWrapper> _mockRepoConfig;
        private readonly RestaurantController _restaurantController;

        public RestaurantControllerTest() {
            _mockRepoReservation = new Mock<IReservationRepository>();
            _mockRepoRestaurant = new Mock<IRestaurantRepository>();
            _mockRepoConfig = new Mock<IConfigurationWrapper>();

            // uncomment the constructor at customercontroller
            LoggerFactory loggerFactory = new LoggerFactory();
            _restaurantController = new RestaurantController(new RestaurantService(_mockRepoRestaurant.Object, _mockRepoConfig.Object), new ReservationService(_mockRepoReservation.Object, _mockRepoRestaurant.Object), loggerFactory);
        }

        [Theory]
        [InlineData(1, 2, -3)]
        [InlineData(1, 2, 0)]
        public void AddRestaurantTable_InvalidData_BadRequest(int restaurantId, int tableNumber, int seats) {
            if (restaurantId > 0) {
                _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(true);
                _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(restaurantId, tableNumber)).Returns(true);
            }

            var result = _restaurantController.AddRestaurantTable(restaurantId, new RestaurantTableRESTinputDTO(tableNumber, seats));
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void AddRestaurantTable_ValidData_CreatedAtAction() {
            int restaurantId = 1;
            int tableNumber = 2;
            int seats = 5;
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(restaurantId, tableNumber)).Returns(false);


            Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
            restaurant.SetRestaurantId(restaurantId);
            //restaurant.Tables.Add(tableNumber, seats);

            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(restaurantId)).Returns(restaurant);
            _mockRepoRestaurant.Setup(repo => repo.GetTables(restaurantId)).Returns(new Dictionary<int, int> { { 2, 5 } });


            var result = _restaurantController.AddRestaurantTable(restaurantId, new RestaurantTableRESTinputDTO(tableNumber, seats));
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Fact]
        public void AddRestaurantTable_ValidData_RestaurantContainsTable() {
            int restaurantId = 1;
            int tableNumber = 2;
            int seats = 5;
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(restaurantId, tableNumber)).Returns(false);


            Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
            restaurant.SetRestaurantId(restaurantId);
            //restaurant.Tables.Add(tableNumber, seats);

            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(restaurantId)).Returns(restaurant);
            _mockRepoRestaurant.Setup(repo => repo.GetTables(restaurantId)).Returns(new Dictionary<int, int> { { 2,5 } });
            

            var result = _restaurantController.AddRestaurantTable(restaurantId, new RestaurantTableRESTinputDTO(tableNumber, seats)) as CreatedAtActionResult;
            Assert.IsType<CreatedAtActionResult>(result);
            Assert.IsType<RestaurantDetailRESToutputDTO>(result.Value);
            Assert.Contains(tableNumber, ((RestaurantDetailRESToutputDTO)result.Value).TablenumberSeats.Keys);
            Assert.Equal(seats, ((RestaurantDetailRESToutputDTO)result.Value).TablenumberSeats[tableNumber]);
        }
    }
}
