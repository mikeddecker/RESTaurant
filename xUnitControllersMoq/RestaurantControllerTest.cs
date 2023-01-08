using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Logging;
using Moq;
using RESTaurant.Controllers;
using RESTaurant.Exceptions;
using RESTaurant.Mappers;
using RESTaurant.Model.Input;
using RESTaurant.Model.Output;
using RESTaurantBL.Interfaces;
using RESTaurantBL.Model;
using RESTaurantBL.Services;
using RESTaurantDLEF.EFModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Table = RESTaurantBL.Model.Table;

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

        #region AddRestaurant
        [Theory]
        [InlineData("cartoon", "buggenhout", 9255, "french", "+32478090859", "info@")]
        [InlineData("cartoon", "buggenhout", 9255, null, "+32478090859", "info@cartoon.be")]
        [InlineData("cartoon", "buggenhout", 9255, "french", " ", "info@cartoon.be")]
        [InlineData("cartoon", "buggenhout", 9255, "french", null, "info@cartoon.be")]
        [InlineData("cartoon", "buggenhout", 55, "french", "+32478090859", "info@cartoon.be")]
        [InlineData("cartoon", "buggenhout", 9999255, "french", "+32478090859", "info@cartoon.be")]
        [InlineData("", "buggenhout", 9255, "french", "+32478090859", "info@cartoon.be")]
        [InlineData(null, "buggenhout", 9255, "french", "+32478090859", "info@cartoon.be")]
        [InlineData("cartoon", null, 9255, "french", "+32478090859", "info@cartoon.be")]
        [InlineData("cartoon", "", 9255, "french", "+32478090859", "info@cartoon.be")]
        public void AddRestaurant_InvalidData_MapException(string name, string city, int postalcode, string kitchen, string phone, string email) {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO(name, email, phone, kitchen, new LocationRESTinputDTO(postalcode, city, null, null));

            Assert.Throws<MapException>(() => MapToDomain.MapRestaurant(restaurantRESTinput));

        }

        [Fact]
        public void AddRestaurant_BadKitchenType_BadRequest() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "smullen", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(restaurantRESTinput);
            _mockRepoConfig.Setup(repo => repo.ContainsKitchenType(restaurant.Kitchen)).Returns(false);
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurant)).Returns(false);
            _mockRepoRestaurant.Setup(repo => repo.AddRestaurant(restaurant)).Returns(restaurant);
            var result = _restaurantController.AddRestaurant(restaurantRESTinput);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void AddRestaurant_RestaurantAlreadyExists_BadRequest() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "smullen", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(restaurantRESTinput);
            _mockRepoConfig.Setup(repo => repo.ContainsKitchenType(restaurant.Kitchen)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurant)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.AddRestaurant(restaurant)).Returns(restaurant);
            var result = _restaurantController.AddRestaurant(restaurantRESTinput);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void AddRestaurant_Valid_CreatedAtActionResult() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(restaurantRESTinput);
            _mockRepoConfig.Setup(repo => repo.ContainsKitchenType(restaurant.Kitchen)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurant)).Returns(false);
            _mockRepoRestaurant.Setup(repo => repo.AddRestaurant(restaurant)).Returns(restaurant);
            var result = _restaurantController.AddRestaurant(restaurantRESTinput);
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public void AddRestaurant_Valid_ReturnsRestaurant() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(restaurantRESTinput);
            _mockRepoConfig.Setup(repo => repo.ContainsKitchenType(restaurant.Kitchen)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurant)).Returns(false);
            _mockRepoRestaurant.Setup(repo => repo.AddRestaurant(restaurant)).Returns(restaurant);
            var result = _restaurantController.AddRestaurant(restaurantRESTinput).Result as CreatedAtActionResult;
            Assert.IsType<RestaurantRESToutputDTO>(result.Value);
            Assert.Equal(restaurant.Kitchen, ((RestaurantRESToutputDTO)result.Value).Kitchen);
            Assert.Equal(restaurant.Name, ((RestaurantRESToutputDTO)result.Value).Name);
            Assert.Equal(restaurant.Location.PostalCode, ((RestaurantRESToutputDTO)result.Value).Location.PostalCode);
        }
        #endregion

        #region GetRestaurants
        [Fact]
        public void GetRestaurants_OkResult() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);
            List<Restaurant> restaurants = new List<Restaurant> { restaurant };
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurants()).Returns(restaurants);
            var result = _restaurantController.GetRestaurants();
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetRestaurants_ReturnsListRestaurantRESToutputDTO() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);
            List<Restaurant> restaurants = new List<Restaurant> { restaurant };
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurants()).Returns(restaurants);
            var result = _restaurantController.GetRestaurants().Result as OkObjectResult;
            Assert.IsType<List<RestaurantRESToutputDTO>>(result.Value);
            Assert.Contains(restaurant.Name, ((List<RestaurantRESToutputDTO>)result.Value).Select(r => r.Name));
        }

        #endregion

        #region GetRestaurant
        [Fact]
        public void GetRestaurant_InvalidId_BadRequest() {
            //_mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(-3)).Returns(false);
            var result = _restaurantController.GetRestaurant(-3);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void GetRestaurant_UnknownId_BadRequest() {
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            var result = _restaurantController.GetRestaurant(3);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void GetRestaurant_Valid_OkObjectResult() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(3)).Returns(restaurant);
            var result = _restaurantController.GetRestaurant(3);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetRestaurant_Valid_ReturnsRestaurantRESToutput() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(3)).Returns(restaurant);
            var result = _restaurantController.GetRestaurant(3).Result as OkObjectResult;
            Assert.IsType<RestaurantRESToutputDTO>(result.Value);
            Assert.Equal(restaurant.Kitchen, ((RestaurantRESToutputDTO)result.Value).Kitchen);
            Assert.Equal(restaurant.Name, ((RestaurantRESToutputDTO)result.Value).Name);
            Assert.Equal(restaurant.Location.PostalCode, ((RestaurantRESToutputDTO)result.Value).Location.PostalCode);
        }
        #endregion

        #region GetRestaurantDetails
        [Fact]
        public void GetRestaurantDetails_InvalidId_BadRequest() {
            //_mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(-3)).Returns(false);
            var result = _restaurantController.GetRestaurantDetails(-3);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void GetRestaurantDetails_UnknownId_BadRequest() {
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            var result = _restaurantController.GetRestaurantDetails(3);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void GetRestaurantDetails_Valid_OkObjectResult() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);

            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(3)).Returns(restaurant);
            _mockRepoRestaurant.Setup(repo => repo.GetTables(3)).Returns(new Dictionary<int, int> { { 2, 2 }, { 3, 4 } });
            var result = _restaurantController.GetRestaurantDetails(3);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetRestaurantDetails_Valid_ReturnsDetailsREST() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);

            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(3)).Returns(restaurant);
            _mockRepoRestaurant.Setup(repo => repo.GetTables(3)).Returns(new Dictionary<int, int> { { 2, 2 }, { 3, 4 } });
            var result = _restaurantController.GetRestaurantDetails(3).Result as OkObjectResult;
            Assert.IsType<RestaurantDetailRESToutputDTO>(result.Value);
            Assert.Contains(3, ((RestaurantDetailRESToutputDTO)result.Value).TablenumberSeats.Keys);
        }

        #endregion

        #region UpdateRestaurant
        [Fact]
        public void UpdateRestaurant_InvalidId_BadRequest() {
            //RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            ////Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);

            //_mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(-3)).Returns(true);
            //_mockRepoRestaurant.Setup(repo => repo.GetRestaurant(3)).Returns(restaurant);
            //_mockRepoRestaurant.Setup(repo => repo.GetTables(3)).Returns(new Dictionary<int, int> { { 2, 2 }, { 3, 4 } });
            var result = _restaurantController.GetRestaurantDetails(-3);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void UpdateRestaurant_UnknownId_BadRequest() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);

            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(false);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(3)).Returns(restaurant);
            _mockRepoRestaurant.Setup(repo => repo.UpdateRestaurant(restaurant)).Returns(restaurant);

            var result = _restaurantController.UpdateRestaurant(3, restaurantRESTinput);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void UpdateRestaurant_NoUpdate_BadRequest() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);

            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(3)).Returns(restaurant);
            _mockRepoRestaurant.Setup(repo => repo.UpdateRestaurant(restaurant)).Returns(restaurant);
            _mockRepoConfig.Setup(repo => repo.ContainsKitchenType(restaurant.Kitchen)).Returns(true);

            var result = _restaurantController.UpdateRestaurant(3, restaurantRESTinput);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("cartoon", "buggenhout", 9255, "french", "+32478090859", "info@cartoon.be", "langeMinnestraat", "9b")]
        [InlineData(null, "buggenhout", 9255, "french", "+32478090859", "info@cartoon.be", "langeMinnestraat", "9b")]
        [InlineData(null, null, 9255, "french", "+32478090859", "info@cartoon.be", "langeMinnestraat", "9b")]
        [InlineData(null, null, 0, "french", "+32478090859", "info@cartoon.be", "langeMinnestraat", "9b")]
        [InlineData(null, null, 0, null, "+32478090859", "info@cartoon.be", "langeMinnestraat", "9b")]
        [InlineData(null, null, 0, null, null, "info@cartoon.be", "langeMinnestraat", "9b")]
        [InlineData(null, null, 0, null, null, null, "langeMinnestraat", "9b")]
        [InlineData("cartoon", null, 0, null, null, null, null, null)]
        [InlineData(null, null, 0, null, null, null, null, "9b")]
        [InlineData(null, null, 0, null, null, null, "langeMinnestraat", null)]
        [InlineData(null, null, 0, null, null, "info@cartoon.be", null, null)]
        [InlineData(null, null, 0, null, "+32478090859", null, null, null)]
        [InlineData(null, null, 0, "french", null, null, null, null)]
        [InlineData(null, null, 9255, null, null, null, null, null)]
        [InlineData(null, "buggenhout", 0, null, null, null, null, null)]
        public void UpdateRestaurant_Valid_CreatedAtActionResult(string name, string city, int postalcode, string kitchen, string phone, string email, string street, string number) {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoonis", "info@cartoon.bel", "+32478090860", "italian", new LocationRESTinputDTO(1745, "Opwijk", "friedastraat", "999"));
            RestaurantRESTinputDTO updaterestaurantRESTinput = new RestaurantRESTinputDTO("Cartoonis", "info@cartoon.bel", "+32478090860", "italian", new LocationRESTinputDTO(1745, "Opwijk", "friedastraat", "999"));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);
            if (name != null) { updaterestaurantRESTinput.Name = name; }
            if (phone != null) { updaterestaurantRESTinput.Phone = phone; }
            if (kitchen != null) { updaterestaurantRESTinput.Kitchen = kitchen; }
            if (email != null) { updaterestaurantRESTinput.Email = email; }
            if (postalcode != 0) { updaterestaurantRESTinput.Location.PostalCode = postalcode; }
            if (street != null) { updaterestaurantRESTinput.Location.Street = street; }
            if (number != null) { updaterestaurantRESTinput.Location.Housenumberlabel = number; }
            if (city != null) { updaterestaurantRESTinput.Location.City = city; }
            Restaurant restaurantUpdate = MapToDomain.MapRestaurant(3, updaterestaurantRESTinput);

            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(3)).Returns(restaurant);
            _mockRepoConfig.Setup(repo => repo.ContainsKitchenType(restaurantUpdate.Kitchen)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.UpdateRestaurant(restaurantUpdate)).Returns(restaurantUpdate);

            var result = _restaurantController.UpdateRestaurant(3, updaterestaurantRESTinput);
            Assert.IsType<CreatedAtActionResult>(result);
        }

        [Theory]
        [InlineData("cartoon", "buggenhout", 9255, "french", "+32478090859", "info@cartoon.be", "langeMinnestraat", "9b")]
        [InlineData(null, "buggenhout", 9255, "french", "+32478090859", "info@cartoon.be", "langeMinnestraat", "9b")]
        [InlineData(null, null, 9255, "french", "+32478090859", "info@cartoon.be", "langeMinnestraat", "9b")]
        [InlineData(null, null, 0, "french", "+32478090859", "info@cartoon.be", "langeMinnestraat", "9b")]
        [InlineData(null, null, 0, null, "+32478090859", "info@cartoon.be", "langeMinnestraat", "9b")]
        [InlineData(null, null, 0, null, null, "info@cartoon.be", "langeMinnestraat", "9b")]
        [InlineData(null, null, 0, null, null, null, "langeMinnestraat", "9b")]
        [InlineData("cartoon", null, 0, null, null, null, null, null)]
        [InlineData(null, null, 0, null, null, null, null, "9b")]
        [InlineData(null, null, 0, null, null, null, "langeMinnestraat", null)]
        [InlineData(null, null, 0, null, null, "info@cartoon.be", null, null)]
        [InlineData(null, null, 0, null, "+32478090859", null, null, null)]
        [InlineData(null, null, 0, "french", null, null, null, null)]
        [InlineData(null, null, 9255, null, null, null, null, null)]
        [InlineData(null, "buggenhout", 0, null, null, null, null, null)]
        public void UpdateRestaurant_Valid_ReturnsRestaurant(string name, string city, int postalcode, string kitchen, string phone, string email, string street, string number) {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoonis", "info@cartoon.bel", "+32478090860", "italian", new LocationRESTinputDTO(1745, "Opwijk", "friedastraat", "999"));
            RestaurantRESTinputDTO updaterestaurantRESTinput = new RestaurantRESTinputDTO("Cartoonis", "info@cartoon.bel", "+32478090860", "italian", new LocationRESTinputDTO(1745, "Opwijk", "friedastraat", "999"));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);
            if (name != null) { updaterestaurantRESTinput.Name = name; }
            if (phone != null) { updaterestaurantRESTinput.Phone = phone; }
            if (kitchen != null) { updaterestaurantRESTinput.Kitchen = kitchen; }
            if (email != null) { updaterestaurantRESTinput.Email = email; }
            if (postalcode != 0) { updaterestaurantRESTinput.Location.PostalCode = postalcode; }
            if (street != null) { updaterestaurantRESTinput.Location.Street = street; }
            if (number != null) { updaterestaurantRESTinput.Location.Housenumberlabel = number; }
            if (city != null) { updaterestaurantRESTinput.Location.City = city; }
            Restaurant restaurantUpdate = MapToDomain.MapRestaurant(3, updaterestaurantRESTinput);

            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(3)).Returns(restaurant);
            _mockRepoConfig.Setup(repo => repo.ContainsKitchenType(restaurantUpdate.Kitchen)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.UpdateRestaurant(restaurantUpdate)).Returns(restaurantUpdate);

            var result = _restaurantController.UpdateRestaurant(3, updaterestaurantRESTinput) as CreatedAtActionResult;
            Assert.IsType<RestaurantRESToutputDTO>(result.Value);
            if (name != null) { Assert.Equal(name, ((RestaurantRESToutputDTO)result.Value).Name); }
            if (phone != null) { Assert.Equal(phone, ((RestaurantRESToutputDTO)result.Value).Phone); }
            if (email != null) { Assert.Equal(email, ((RestaurantRESToutputDTO)result.Value).Email); }
            if (kitchen != null) { Assert.Equal(kitchen, ((RestaurantRESToutputDTO)result.Value).Kitchen); }
            if (postalcode != 0) { Assert.Equal(postalcode, ((RestaurantRESToutputDTO)result.Value).Location.PostalCode); }
            if (city != null) { Assert.Equal("Buggenhout", ((RestaurantRESToutputDTO)result.Value).Location.City); }
            if (street != null) { Assert.Equal("Langeminnestraat", ((RestaurantRESToutputDTO)result.Value).Location.Street); }
            if (number != null) { Assert.Equal(number, ((RestaurantRESToutputDTO)result.Value).Location.Housenumberlabel); }
        }

        #endregion

        #region DeleteRestaurant
        [Fact]
        public void DeleteRestaurant_InvalidID_BadRequest() {
            var result = _restaurantController.DeleteRestaurant(0);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void DeleteRestaurant_UnknownID_BadRequest() {
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(false);
            var result = _restaurantController.DeleteRestaurant(3);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void DeleteRestaurant_KnownID_NoContentResult() {
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            var result = _restaurantController.DeleteRestaurant(3);
            Assert.IsType<NoContentResult>(result);
        }

        #endregion

        #region AddRestaurantTable
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
            _mockRepoRestaurant.Setup(repo => repo.GetTables(restaurantId)).Returns(new Dictionary<int, int> { { 2, 5 } });


            var result = _restaurantController.AddRestaurantTable(restaurantId, new RestaurantTableRESTinputDTO(tableNumber, seats)) as CreatedAtActionResult;
            Assert.IsType<CreatedAtActionResult>(result);
            Assert.IsType<RestaurantDetailRESToutputDTO>(result.Value);
            Assert.Contains(tableNumber, ((RestaurantDetailRESToutputDTO)result.Value).TablenumberSeats.Keys);
            Assert.Equal(seats, ((RestaurantDetailRESToutputDTO)result.Value).TablenumberSeats[tableNumber]);
        }
        #endregion

        #region UpdateTableOfRestaurant

        [Fact]
        public void UpdateTableOfRestaurant_InvalidRestaurantId_BadRequest() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);
            RestaurantTableRESTinputDTO table = new RestaurantTableRESTinputDTO(3, 2);
            Table tDB = new Table(table.TableNumber, 4);
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(false);
            _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(3, table.TableNumber)).Returns(false);
            _mockRepoRestaurant.Setup(repo => repo.GetTable(3, table.TableNumber)).Returns(tDB);
            _mockRepoRestaurant.Setup(repo => repo.GetTables(3)).Returns(new Dictionary<int, int> { { 2, 2 }, { table.TableNumber, table.Seats } });
            var result = _restaurantController.UpdateTableOfRestaurant(-3, table);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void UpdateTableOfRestaurant_UnknownRestaurantId_BadRequest() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);
            RestaurantTableRESTinputDTO table = new RestaurantTableRESTinputDTO(3, 2);
            Table tDB = new Table(table.TableNumber, 4);
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(false);
            _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(3, table.TableNumber)).Returns(false);
            _mockRepoRestaurant.Setup(repo => repo.GetTable(3, table.TableNumber)).Returns(tDB);
            _mockRepoRestaurant.Setup(repo => repo.GetTables(3)).Returns(new Dictionary<int, int> { { 2, 2 }, { table.TableNumber, table.Seats } });
            //_mockRepoRestaurant.Setup(repo => repo.GetRestaurant(3)).Returns(restaurant);
            var result = _restaurantController.UpdateTableOfRestaurant(3, table);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void UpdateTableOfRestaurant_InvalidSeats_BadRequest() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);
            RestaurantTableRESTinputDTO table = new RestaurantTableRESTinputDTO(3, -2);
            Table tDB = new Table(table.TableNumber, 4);
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(3, table.TableNumber)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.GetTable(3, table.TableNumber)).Returns(tDB);
            _mockRepoRestaurant.Setup(repo => repo.GetTables(3)).Returns(new Dictionary<int, int> { { 2, 2 }, { table.TableNumber, table.Seats } });
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(3)).Returns(restaurant);
            var result = _restaurantController.UpdateTableOfRestaurant(3, table);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void UpdateTableOfRestaurant_RestaurantHasNotTable_BadRequest() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);
            RestaurantTableRESTinputDTO table = new RestaurantTableRESTinputDTO(3, -2);
            Table tDB = new Table(table.TableNumber, 4);
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(3, table.TableNumber)).Returns(false);
            _mockRepoRestaurant.Setup(repo => repo.GetTable(3, table.TableNumber)).Returns(tDB);
            _mockRepoRestaurant.Setup(repo => repo.GetTables(3)).Returns(new Dictionary<int, int> { { 2, 2 }, { table.TableNumber, table.Seats } });
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(3)).Returns(restaurant);
            var result = _restaurantController.UpdateTableOfRestaurant(3, table);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void UpdateTableOfRestaurant_NoUpdate_BadRequest() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);
            RestaurantTableRESTinputDTO table = new RestaurantTableRESTinputDTO(3, 4);
            Table tDB = new Table(table.TableNumber, 4);
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(3, table.TableNumber)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.GetTable(3, table.TableNumber)).Returns(tDB);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(3)).Returns(restaurant);
            _mockRepoRestaurant.Setup(repo => repo.GetTables(3)).Returns(new Dictionary<int, int> { { 2, 2 }, { table.TableNumber, table.Seats } });
            var result = _restaurantController.UpdateTableOfRestaurant(3, table);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void UpdateTableeOfRestaurant_Valid_CreatedAtAction() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);
            RestaurantTableRESTinputDTO table = new RestaurantTableRESTinputDTO(3, 2);
            Table tDB = new Table(table.TableNumber, 4);
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(3, table.TableNumber)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.GetTable(3, table.TableNumber)).Returns(tDB);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(3)).Returns(restaurant);
            _mockRepoRestaurant.Setup(repo => repo.GetTables(3)).Returns(new Dictionary<int, int> { { 2, 2 }, { table.TableNumber, table.Seats } });
            var result = _restaurantController.UpdateTableOfRestaurant(3, table);
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public void UpdateTableOfRestaurant_Valid_ReturnsRestaurantDetails() {
            RestaurantRESTinputDTO restaurantRESTinput = new RestaurantRESTinputDTO("Cartoon", "info@cartoon.be", "+32478090859", "french", new LocationRESTinputDTO(9255, "Buggenhout", null, null));
            Restaurant restaurant = MapToDomain.MapRestaurant(3, restaurantRESTinput);
            RestaurantTableRESTinputDTO table = new RestaurantTableRESTinputDTO(3, 2);
            Table tDB = new Table(table.TableNumber, 4);
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(3, table.TableNumber)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.GetTable(3, table.TableNumber)).Returns(tDB);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(3)).Returns(restaurant);
            _mockRepoRestaurant.Setup(repo => repo.GetTables(3)).Returns(new Dictionary<int, int> { { 2, 2 }, { table.TableNumber, table.Seats } });
            var result = _restaurantController.UpdateTableOfRestaurant(3, table).Result as CreatedAtActionResult;
            Assert.IsType<RestaurantDetailRESToutputDTO>(result.Value);
            Assert.Contains(table.TableNumber, ((RestaurantDetailRESToutputDTO)result.Value).TablenumberSeats.Keys);
            Assert.Equal(table.Seats, ((RestaurantDetailRESToutputDTO)result.Value).TablenumberSeats[table.TableNumber]);
        }


        #endregion

        #region DeleteTableRestaurant
        [Fact]
        public void DeleteTable_InvalidId_NotFound() {
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(3, 5)).Returns(true);
            var result = _restaurantController.DeleteTableRestaurant(-3, 5);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void DeleteTable_UnknownId_NotFound() {
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(false);
            _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(3, 5)).Returns(true);
            var result = _restaurantController.DeleteTableRestaurant(3, 5);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void DeleteTable_RestaurantHasNotTable_NotFound() {
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(3, 5)).Returns(false);
            var result = _restaurantController.DeleteTableRestaurant(3, 5);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void DeleteTable_Valid_NoContent() {
            _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(3)).Returns(true);
            _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(3, 5)).Returns(true);
            var result = _restaurantController.DeleteTableRestaurant(3, 5);
            Assert.IsType<BadRequestObjectResult>(result);
        }
        #endregion

        #region GetReservations
        [Theory]
        [InlineData(-1, null, null)]
        [InlineData(0, null, null)]
        [InlineData(1, 2050, 2020)]
        public void GetReservations_InvalidData_BadRequest(int restaurantId, int? startYear, int? endyear) {
            DateTime? day = startYear.HasValue ? new DateTime(startYear.Value, 01, 01) : null;
            DateTime? endTime = endyear.HasValue ? new DateTime(endyear.Value, 12, 31) : null;
            if (restaurantId > 0 && day.HasValue && endTime.HasValue) {
                Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
                customer.SetCustomerId(111);
                Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
                restaurant.SetRestaurantId(restaurantId);
                Reservation reservation = new Reservation(restaurant, customer, new Table(3, 4), 4, new DateTime(2030, 2, 2, 2, 30, 0));
                List<Reservation> reservations = new List<Reservation>();
                reservations.Add(reservation);
                if (day.HasValue && endTime.HasValue) {
                    _mockRepoReservation.Setup(repo => repo.GetReservations(restaurantId, day.Value, endTime.Value)).Returns(reservations);
                } else if (endTime.HasValue) {
                    _mockRepoReservation.Setup(repo => repo.GetReservations(restaurantId, new DateTime(1900, 01, 01), endTime.Value)).Returns(reservations);
                } else {
                    _mockRepoReservation.Setup(repo => repo.GetReservations(restaurantId, day.Value, new DateTime(2999, 01, 01))).Returns(reservations);

                }
            }

            var result = _restaurantController.GetReservations(restaurantId, day, endTime);
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Theory]
        [InlineData(1, null, null)]
        [InlineData(1, 2020, null)]
        [InlineData(1, 2020, 2050)]
        [InlineData(1, null, 2050)]
        public void GetReservations_ValidData_OkResult(int restaurantId, int? startYear, int? endyear) {
            DateTime? day = startYear.HasValue ? new DateTime(startYear.Value, 01, 01) : null;
            DateTime? endTime = endyear.HasValue ? new DateTime(endyear.Value, 12, 31) : null;
            if (restaurantId > 0) {
                Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
                customer.SetCustomerId(111);
                Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
                if (restaurantId > 0) { restaurant.SetRestaurantId(restaurantId); }
                Reservation reservation = new Reservation(restaurant, customer, new Table(3, 4), 4, new DateTime(2030, 2, 2, 2, 30, 0));
                List<Reservation> reservations = new List<Reservation>();
                reservations.Add(reservation);
                if (day.HasValue && endTime.HasValue) {
                    _mockRepoReservation.Setup(repo => repo.GetReservations(restaurantId, day.Value, endTime.Value)).Returns(reservations);
                } else if (endTime.HasValue) {
                    _mockRepoReservation.Setup(repo => repo.GetReservations(restaurantId, new DateTime(1900, 01, 01), endTime.Value)).Returns(reservations);
                } else if (day.HasValue) {
                    _mockRepoReservation.Setup(repo => repo.GetReservations(restaurantId, day.Value, new DateTime(2999, 12, 31))).Returns(reservations);
                } else {
                    _mockRepoReservation.Setup(repo => repo.GetReservations(restaurantId, new DateTime(1900, 01, 01), new DateTime(2999, 12, 31))).Returns(reservations);
                }
            }

            var result = _restaurantController.GetReservations(restaurantId, day, endTime);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Theory]
        [InlineData(1, null, null)]
        [InlineData(1, 2020, null)]
        [InlineData(1, 2020, 2050)]
        [InlineData(1, null, 2050)]
        public void GetReservations_ValidData_ListOfRESTReservation(int restaurantId, int? startYear, int? endyear) {
            DateTime? day = startYear.HasValue ? new DateTime(startYear.Value, 01, 01) : null;
            DateTime? endTime = endyear.HasValue ? new DateTime(endyear.Value, 12, 31) : null;
            if (restaurantId > 0) {
                Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
                customer.SetCustomerId(111);
                Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
                restaurant.SetRestaurantId(1);
                Reservation reservation = new Reservation(restaurant, customer, new Table(3, 4), 4, new DateTime(2030, 2, 2, 2, 30, 0));
                List<Reservation> reservations = new List<Reservation>();
                reservations.Add(reservation);
                if (day.HasValue && endTime.HasValue) {
                    _mockRepoReservation.Setup(repo => repo.GetReservations(restaurantId, day.Value, endTime.Value)).Returns(reservations);
                } else if (endTime.HasValue) {
                    _mockRepoReservation.Setup(repo => repo.GetReservations(restaurantId, new DateTime(1900, 01, 01), endTime.Value)).Returns(reservations);
                } else if (day.HasValue) {
                    _mockRepoReservation.Setup(repo => repo.GetReservations(restaurantId, day.Value, new DateTime(2999, 12, 31))).Returns(reservations);
                } else {
                    _mockRepoReservation.Setup(repo => repo.GetReservations(restaurantId, new DateTime(1900, 01, 01), new DateTime(2999, 12, 31))).Returns(reservations);
                }
            }

            var result = _restaurantController.GetReservations(restaurantId, day, endTime);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        DateTime dateHash0;
        [Fact]
        public void GetReservations_InvalidData_DateHashCode0_BadRequest() {
            var result1 = _restaurantController.GetReservations(1, dateHash0, null);
            var result2 = _restaurantController.GetReservations(1, dateHash0, dateHash0);
            var result3 = _restaurantController.GetReservations(1, null, dateHash0);
            Assert.IsType<NotFoundObjectResult>(result1.Result);
            Assert.IsType<NotFoundObjectResult>(result2.Result);
            Assert.IsType<NotFoundObjectResult>(result3.Result);
        }
        #endregion
    }
}
