using Moq;
using RESTaurantBL.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using RESTaurant.Exceptions;
using RESTaurant.Controllers;
using RESTaurantBL.Services;
using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using RESTaurantBL.Model;
using RESTaurant.Model.Output;
using RESTaurant.Model.Input;
using RESTaurant.Mappers;
using Microsoft.AspNetCore.Mvc;
using Xunit.Sdk;
using RESTaurantDLEF.EFModel;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Table = RESTaurantBL.Model.Table;
using Microsoft.EntityFrameworkCore.Update;

namespace xUnitControllersMoq {
    public class CustomerControlerTest {
        private readonly Mock<IReservationRepository> _mockRepoReservation;
        private readonly Mock<IRestaurantRepository> _mockRepoRestaurant;
        private readonly Mock<ICustomerRepository> _mockRepoCustomer;
        private readonly Mock<IConfigurationWrapper> _mockRepoConfig;
        private readonly CustomerController _customerController;

        public CustomerControlerTest() {
            _mockRepoReservation = new Mock<IReservationRepository>();
            _mockRepoRestaurant = new Mock<IRestaurantRepository>();
            _mockRepoCustomer = new Mock<ICustomerRepository>();
            _mockRepoConfig = new Mock<IConfigurationWrapper>();

            // uncomment the constructor at customercontroller
            LoggerFactory loggerFactory = new LoggerFactory();
            _customerController = new CustomerController(new CustomerService(_mockRepoCustomer.Object), new RestaurantService(_mockRepoRestaurant.Object, _mockRepoConfig.Object), new ReservationService(_mockRepoReservation.Object, _mockRepoRestaurant.Object), loggerFactory);
        }

        #region AddCustomer
        [Theory]
        [InlineData("Mike", "info@mike.be", "+32477486852", 9255, "Buggenhout")]
        [InlineData("  ", "info@mike.be", "+32477486852", 9255, "Buggenhout")]
        [InlineData(null, "info@mike.be", "+32477486852", 9255, "Buggenhout")]
        [InlineData("Mike", "@m.be", "+32477486852", 9255, "Buggenhout")]
        [InlineData("Mike", null, "+32477486852", 9255, "Buggenhout")]
        [InlineData("Mike", "info@mike.be", "04 zie je van hier", 9255, "Buggenhout")]
        [InlineData("Mike", "info@mike.be", null, 9255, "Buggenhout")]
        [InlineData("Mike", "info@mike.be", "+32477486852", 925, "Buggenhout")]
        [InlineData("Mike", "info@mike.be", "+32477486852", 10005, "Buggenhout")]
        [InlineData("Mike", "info@mike.be", "+32477486852", 9255, " ")]
        [InlineData("Mike", "info@mike.be", "+32477486852", 9255, null)]
        public void AddCustomer_InvalidCustomerData_BadRequest(string name, string email, string phone, int postalcode, string city) {
            LocationRESTinputDTO locationInput = new LocationRESTinputDTO(postalcode, city, null, null);
            CustomerRESTinputDTO customerInput = new CustomerRESTinputDTO(name, email, phone, locationInput);


            var result = _customerController.AddCustomer(customerInput);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void AddCustomer_validData_CreatedAtActionResult() {
            LocationRESTinputDTO locationInput = new LocationRESTinputDTO(9255, "Buggenhout", null, null);
            CustomerRESTinputDTO customerInput = new CustomerRESTinputDTO("Mike", "info@mike.be", "+32478090859", locationInput);

            Customer customer = MapToDomain.MapCustomer(customerInput);

            _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(customer)).Returns(false);
            //customer.SetCustomerId(3);
            _mockRepoCustomer.Setup(repo => repo.AddCustomer(customer)).Returns(customer);
            var result = _customerController.AddCustomer(customerInput);
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public void AddCustomer_ValidData_ReturnsCustomer() {
            LocationRESTinputDTO locationInput = new LocationRESTinputDTO(9255, "Buggenhout", null, null);
            CustomerRESTinputDTO customerInput = new CustomerRESTinputDTO("Mike", "info@mike.be", "+32478090859", locationInput);

            Customer customer = MapToDomain.MapCustomer(customerInput);
            //customer.SetCustomerId(3);

            CustomerRESToutputDTO customerOutput = MapToREST.MapCustomer("localhost", customer);

            _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(customer)).Returns(false);
            _mockRepoCustomer.Setup(repo => repo.AddCustomer(customer)).Returns(customer);
            var result = _customerController.AddCustomer(customerInput).Result as CreatedAtActionResult;
            Assert.IsType<CustomerRESToutputDTO>(result.Value);
            // Testing a few
            //Assert.Contains("3", customerOutput.CustomerID);
            Assert.Equal(customerInput.Name, customerOutput.Name);
            Assert.Equal(customerInput.Phone, customerOutput.Phone);
            Assert.Equal(customerInput.Email, customerOutput.Email);
            Assert.Equal(customerInput.Location.PostalCode, customerOutput.Location.PostalCode);
            Assert.Equal(customerInput.Location.City, customerOutput.Location.City);
        }
        #endregion

        #region GetCustomer
        [Fact]
        public void GetCustomer_UnknownID_NotFound() {
            _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(3)).Returns(false);
            //_mockRepoCustomer.Setup(repo => repo.GetCustomer(3)).Throws(() => new CustomerRepoException("GetCustomer"));
            var result = _customerController.GetCustomer(3);

            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public void GetCustomer_InvalidID_BadRequest() {
            var result = _customerController.GetCustomer(-3);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void GetCustomer_knownID_ReturnsOkResult() {
            Customer mike = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
            mike.SetCustomerId(3);

            _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(3)).Returns(true);
            _mockRepoCustomer.Setup(repo => repo.GetCustomer(3)).Returns(mike);

            var result = _customerController.GetCustomer(3);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetCustomer_knownID_ReturnsCustomer() {
            Customer mike = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
            mike.SetCustomerId(1234);

            _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(1234)).Returns(true);
            _mockRepoCustomer.Setup(repo => repo.GetCustomer(1234)).Returns(mike);

            var result = _customerController.GetCustomer(1234).Result as OkObjectResult;
            Assert.IsType<CustomerRESToutputDTO>(result.Value);
            CustomerRESToutputDTO customer = (CustomerRESToutputDTO)result.Value;
            Assert.Contains(mike.CustomerId.ToString(), customer.CustomerID);
            Assert.Equal(mike.Name, customer.Name);
            Assert.Equal(mike.Location.PostalCode, customer.Location.PostalCode);
        }
        #endregion

        #region GetCustomers
        // Not testable
        #endregion

        #region UpdateCustomer
        [Fact]
        public void UpdateCustomer_UnknownID_BadRequest() {
            LocationRESTinputDTO locationInput = new LocationRESTinputDTO(9255, "Buggenhout", null, null);
            CustomerRESTinputDTO customerInput = new CustomerRESTinputDTO("Mike", "info@mike.be", "+32478090859", locationInput);

            Customer customer = MapToDomain.MapCustomer(3, customerInput);

            _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(3)).Returns(false);
            var result = _customerController.UpdateCustomer(3, customerInput);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void UpdateCustomer_InvalidID_BadRequest() {
            var result = _customerController.GetCustomer(-3);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void UpdateCustomer_TheSameProperties_BadRequest() {
            LocationRESTinputDTO locationInput = new LocationRESTinputDTO(9255, "Buggenhout", null, null);
            CustomerRESTinputDTO customerInput = new CustomerRESTinputDTO("Mike", "info@mike.be", "+32478090859", locationInput);

            Customer customer = MapToDomain.MapCustomer(3, customerInput);

            _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(3)).Returns(true);
            _mockRepoCustomer.Setup(repo => repo.GetCustomer(3)).Returns(customer);
            _mockRepoCustomer.Setup(repo => repo.UpdateCustomer(customer)).Returns(customer);
            var result = _customerController.UpdateCustomer(3, customerInput);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void UpdateCustomer_KnownID_ReturnsCreatedAtActionResult() {
            LocationRESTinputDTO locationInput = new LocationRESTinputDTO(9255, "Buggenhout", null, null);
            CustomerRESTinputDTO customerInput = new CustomerRESTinputDTO("Mike", "info@mike.be", "+32478090859", locationInput);

            Customer customer = MapToDomain.MapCustomer(3, customerInput);
            Customer customer2 = new Customer("Mike", "no@email.com", "+32478090859", new Location(9255, "Bug"));

            _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(3)).Returns(true);
            _mockRepoCustomer.Setup(repo => repo.GetCustomer(3)).Returns(customer2);
            _mockRepoCustomer.Setup(repo => repo.UpdateCustomer(customer)).Returns(customer);
            var result = _customerController.UpdateCustomer(3, customerInput);

            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public void UpdateCustomer_KnownID_ReturnsCustomer() {
            LocationRESTinputDTO locationInput = new LocationRESTinputDTO(9255, "Buggenhout", null, null);
            CustomerRESTinputDTO customerInput = new CustomerRESTinputDTO("Mike", "info@mike.be", "+32478090859", locationInput);

            Customer customer = MapToDomain.MapCustomer(3, customerInput);
            Customer customer2 = new Customer("Mike", "no@email.com", "+32478090859", new Location(9255, "Bug"));

            _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(3)).Returns(true);
            _mockRepoCustomer.Setup(repo => repo.GetCustomer(3)).Returns(customer2);
            _mockRepoCustomer.Setup(repo => repo.UpdateCustomer(customer)).Returns(customer);
            var result = _customerController.UpdateCustomer(3, customerInput).Result as CreatedAtActionResult;

            Assert.IsType<CustomerRESToutputDTO>(result.Value);
            CustomerRESToutputDTO customerOutput = (CustomerRESToutputDTO)result.Value;
            Assert.Contains(customer.CustomerId.ToString(), customerOutput.CustomerID);
            Assert.Equal(customer.Name, customerOutput.Name);
            Assert.Equal(customer.Location.PostalCode, customerOutput.Location.PostalCode);
        }
        #endregion

        #region DeleteCustomer
        [Fact]
        public void DeleteCustomer_UnknownID_BadRequest() {
            _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(3)).Returns(false);
            var result = _customerController.DeleteCustomer(3);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void DeleteCustomer_InvalidID_BadRequest() {
            var result = _customerController.GetCustomer(-3);

            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void DeleteCustomer_KnownID_ReturnsNoContentResult() {
            _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(3)).Returns(true);
            var result = _customerController.DeleteCustomer(3);

            Assert.IsType<NoContentResult>(result);
        }
        #endregion

        #region AddReservation

        [Theory]
        [InlineData(-1, 1, 2025, 30, 4)]
        [InlineData(1, -1, 2025, 30, 4)]
        [InlineData(0, 1, 2025, 30, 4)]
        [InlineData(1, 1, 2002, 30, 4)]
        [InlineData(1, 1, 2025, 23, 4)]
        [InlineData(1, 1, 2025, 30, -4)]
        [InlineData(1, 1, 2025, 30, 0)]
        public void AddReservation_InvalidData_BadRequest(int restaurantId, int customerId, int year, int minutes, int seats) {
            DateTime reservationDate = new DateTime(year, 03, 21, 21, minutes, 00);
            ReservationRESTinputDTO reservationRESTinput = new ReservationRESTinputDTO(restaurantId, customerId, seats, reservationDate);

            Table table = new Table(3, 4);
            if (restaurantId > 0 && seats > 0) {
                _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(restaurantId, reservationDate, seats)).Returns(table);
            }
            Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
            if (restaurantId > 0) {
                restaurant.SetRestaurantId(restaurantId);
                _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(true);
                _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(restaurantId)).Returns(restaurant);
                _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(restaurantId, 3)).Returns(true);
                _mockRepoRestaurant.Setup(repo => repo.GetTable(restaurantId, 3)).Returns(new Table(3, 4));
            } else {
                _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(false);
            }

            Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
            if (customerId > 0) {
                customer.SetCustomerId(customerId);
                _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(customerId)).Returns(true);
                _mockRepoCustomer.Setup(repo => repo.GetCustomer(customerId)).Returns(customer);
            } else {
                _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(customerId)).Returns(false);
            }

            if (reservationDate > DateTime.Now && restaurantId > 0 && customerId > 0 && seats > 0) {
                Reservation reservation = new Reservation(restaurant, customer, table, seats, reservationDate);
                _mockRepoReservation.Setup(repo => repo.DoesReservationExist(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.DoesReservationOverlapCustomer(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.DoesReservationOverlapTable(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.AddReservation(reservation)).Returns(reservation);

            }


            var result = _customerController.AddReservation(reservationRESTinput);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void AddReservation_InvalidData_RestaurantHasNotTableNumber_BadRequest() {
            int restaurantId = 22;
            int customerId = 222;
            int seats = 4;
            DateTime reservationDate = new DateTime(2025, 03, 21, 21, 30, 00);
            ReservationRESTinputDTO reservationRESTinput = new ReservationRESTinputDTO(restaurantId, customerId, seats, reservationDate);

            Table table = new Table(3, 4);
            if (restaurantId > 0 && seats > 0) {
                _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(restaurantId, reservationDate, seats)).Returns(table);
            }
            Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
            if (restaurantId > 0) {
                restaurant.SetRestaurantId(restaurantId);
                _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(true);
                _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(restaurantId)).Returns(restaurant);
                _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(restaurantId, 3)).Returns(false); // HERE
                _mockRepoRestaurant.Setup(repo => repo.GetTable(restaurantId, 3)).Returns(new Table(3, 4));
            } else {
                _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(false);
            }

            Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
            if (customerId > 0) {
                customer.SetCustomerId(customerId);
                _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(customerId)).Returns(true);
                _mockRepoCustomer.Setup(repo => repo.GetCustomer(customerId)).Returns(customer);
            } else {
                _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(customerId)).Returns(false);
            }

            if (reservationDate > DateTime.Now && restaurantId > 0 && customerId > 0 && seats > 0) {
                Reservation reservation = new Reservation(restaurant, customer, table, seats, reservationDate);
                _mockRepoReservation.Setup(repo => repo.DoesReservationExist(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.DoesReservationOverlapCustomer(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.DoesReservationOverlapTable(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.AddReservation(reservation)).Returns(reservation);

            }

            var result = _customerController.AddReservation(reservationRESTinput);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void AddReservation_ReservationExists_BadRequest() {
            int restaurantId = 22;
            int customerId = 222;
            int seats = 4;
            DateTime reservationDate = new DateTime(2025, 03, 21, 21, 30, 00);
            ReservationRESTinputDTO reservationRESTinput = new ReservationRESTinputDTO(restaurantId, customerId, seats, reservationDate);

            Table table = new Table(3, 4);
            if (restaurantId > 0 && seats > 0) {
                _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(restaurantId, reservationDate, seats)).Returns(table);
            }
            Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
            if (restaurantId > 0) {
                restaurant.SetRestaurantId(restaurantId);
                _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(true);
                _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(restaurantId)).Returns(restaurant);
                _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(restaurantId, 3)).Returns(true);
                _mockRepoRestaurant.Setup(repo => repo.GetTable(restaurantId, 3)).Returns(new Table(3, 4));
            } else {
                _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(false);
            }

            Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
            if (customerId > 0) {
                customer.SetCustomerId(customerId);
                _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(customerId)).Returns(true);
                _mockRepoCustomer.Setup(repo => repo.GetCustomer(customerId)).Returns(customer);
            } else {
                _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(customerId)).Returns(false);
            }

            if (reservationDate > DateTime.Now && restaurantId > 0 && customerId > 0 && seats > 0) {
                Reservation reservation = new Reservation(restaurant, customer, table, seats, reservationDate);
                _mockRepoReservation.Setup(repo => repo.DoesReservationExist(reservation)).Returns(true); // HERE
                _mockRepoReservation.Setup(repo => repo.DoesReservationOverlapCustomer(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.DoesReservationOverlapTable(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.AddReservation(reservation)).Returns(reservation);

            }

            var result = _customerController.AddReservation(reservationRESTinput);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void AddReservation_CustomerAlreadyHasReservation_BadRequest() {
            int restaurantId = 22;
            int customerId = 222;
            int seats = 4;
            DateTime reservationDate = new DateTime(2025, 03, 21, 21, 30, 00);
            ReservationRESTinputDTO reservationRESTinput = new ReservationRESTinputDTO(restaurantId, customerId, seats, reservationDate);

            Table table = new Table(3, 4);
            if (restaurantId > 0 && seats > 0) {
                _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(restaurantId, reservationDate, seats)).Returns(table);
            }
            Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
            if (restaurantId > 0) {
                restaurant.SetRestaurantId(restaurantId);
                _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(true);
                _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(restaurantId)).Returns(restaurant);
                _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(restaurantId, 3)).Returns(true);
                _mockRepoRestaurant.Setup(repo => repo.GetTable(restaurantId, 3)).Returns(new Table(3, 4));
            } else {
                _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(false);
            }

            Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
            if (customerId > 0) {
                customer.SetCustomerId(customerId);
                _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(customerId)).Returns(true);
                _mockRepoCustomer.Setup(repo => repo.GetCustomer(customerId)).Returns(customer);
            } else {
                _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(customerId)).Returns(false);
            }

            if (reservationDate > DateTime.Now && restaurantId > 0 && customerId > 0 && seats > 0) {
                Reservation reservation = new Reservation(restaurant, customer, table, seats, reservationDate);
                _mockRepoReservation.Setup(repo => repo.DoesReservationExist(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.DoesReservationOverlapCustomer(reservation)).Returns(true);
                _mockRepoReservation.Setup(repo => repo.DoesReservationOverlapTable(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.AddReservation(reservation)).Returns(reservation);

            }


            var result = _customerController.AddReservation(reservationRESTinput);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void AddReservation_TableAlreadyHasReservation_BadRequest() {
            int restaurantId = 22;
            int customerId = 222;
            int seats = 4;
            DateTime reservationDate = new DateTime(2025, 03, 21, 21, 30, 00);
            ReservationRESTinputDTO reservationRESTinput = new ReservationRESTinputDTO(restaurantId, customerId, seats, reservationDate);

            Table table = new Table(3, 4);
            if (restaurantId > 0 && seats > 0) {
                _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(restaurantId, reservationDate, seats)).Returns(table);
            }
            Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
            if (restaurantId > 0) {
                restaurant.SetRestaurantId(restaurantId);
                _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(true);
                _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(restaurantId)).Returns(restaurant);
                _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(restaurantId, 3)).Returns(true);
                _mockRepoRestaurant.Setup(repo => repo.GetTable(restaurantId, 3)).Returns(new Table(3, 4));
            } else {
                _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(false);
            }

            Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
            if (customerId > 0) {
                customer.SetCustomerId(customerId);
                _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(customerId)).Returns(true);
                _mockRepoCustomer.Setup(repo => repo.GetCustomer(customerId)).Returns(customer);
            } else {
                _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(customerId)).Returns(false);
            }

            if (reservationDate > DateTime.Now && restaurantId > 0 && customerId > 0 && seats > 0) {
                Reservation reservation = new Reservation(restaurant, customer, table, seats, reservationDate);
                _mockRepoReservation.Setup(repo => repo.DoesReservationExist(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.DoesReservationOverlapCustomer(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.DoesReservationOverlapTable(reservation)).Returns(true); // HERE
                _mockRepoReservation.Setup(repo => repo.AddReservation(reservation)).Returns(reservation);

            }


            var result = _customerController.AddReservation(reservationRESTinput);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void AddReservation_ValidData_NoTable_BadRequest() {
            int restaurantId = 22;
            int customerId = 222;
            int seats = 4;

            DateTime reservationDate = new DateTime(2025, 03, 21, 21, 00, 00);
            ReservationRESTinputDTO reservationRESTinput = new ReservationRESTinputDTO(restaurantId, customerId, seats, reservationDate);


            Table table = null;
            if (restaurantId > 0 && seats > 0) {
                _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(restaurantId, reservationDate, seats)).Returns(table);
            }

            var result = _customerController.AddReservation(reservationRESTinput);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void AddReservation_Valid_CreatedAtActionResult() {
            int restaurantId = 22;
            int customerId = 222;
            int seats = 4;
            DateTime reservationDate = new DateTime(2025, 03, 21, 21, 30, 00);
            ReservationRESTinputDTO reservationRESTinput = new ReservationRESTinputDTO(restaurantId, customerId, seats, reservationDate);

            Table table = new Table(3, 4);
            if (restaurantId > 0 && seats > 0) {
                _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(restaurantId, reservationDate, seats)).Returns(table);
            }
            Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
            if (restaurantId > 0) {
                restaurant.SetRestaurantId(restaurantId);
                _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(true);
                _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(restaurantId)).Returns(restaurant);
                _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(restaurantId, 3)).Returns(true);
                _mockRepoRestaurant.Setup(repo => repo.GetTable(restaurantId, 3)).Returns(new Table(3, 4));
            } else {
                _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(false);
            }

            Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
            if (customerId > 0) {
                customer.SetCustomerId(customerId);
                _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(customerId)).Returns(true);
                _mockRepoCustomer.Setup(repo => repo.GetCustomer(customerId)).Returns(customer);
            } else {
                _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(customerId)).Returns(false);
            }

            if (reservationDate > DateTime.Now && restaurantId > 0 && customerId > 0 && seats > 0) {
                Reservation reservation = new Reservation(restaurant, customer, table, seats, reservationDate);
                _mockRepoReservation.Setup(repo => repo.DoesReservationExist(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.DoesReservationOverlapCustomer(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.DoesReservationOverlapTable(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.AddReservation(reservation)).Returns(reservation);

            }

            var result = _customerController.AddReservation(reservationRESTinput);
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Fact]
        public void AddReservation_Valid_ReturnsReservation() {
            int restaurantId = 22;
            int customerId = 222;
            int seats = 4;
            DateTime reservationDate = new DateTime(2025, 03, 21, 21, 30, 00);
            ReservationRESTinputDTO reservationRESTinput = new ReservationRESTinputDTO(restaurantId, customerId, seats, reservationDate);

            Table table = new Table(3, 4);
            if (restaurantId > 0 && seats > 0) {
                _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(restaurantId, reservationDate, seats)).Returns(table);
            }
            Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
            if (restaurantId > 0) {
                restaurant.SetRestaurantId(restaurantId);
                _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(true);
                _mockRepoRestaurant.Setup(repo => repo.GetRestaurant(restaurantId)).Returns(restaurant);
                _mockRepoRestaurant.Setup(repo => repo.HasRestaurantTableNumber(restaurantId, 3)).Returns(true);
                _mockRepoRestaurant.Setup(repo => repo.GetTable(restaurantId, 3)).Returns(new Table(3, 4));
            } else {
                _mockRepoRestaurant.Setup(repo => repo.DoesRestaurantExist(restaurantId)).Returns(false);
            }

            Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
            if (customerId > 0) {
                customer.SetCustomerId(customerId);
                _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(customerId)).Returns(true);
                _mockRepoCustomer.Setup(repo => repo.GetCustomer(customerId)).Returns(customer);
            } else {
                _mockRepoCustomer.Setup(repo => repo.DoesCustomerExist(customerId)).Returns(false);
            }

            if (reservationDate > DateTime.Now && restaurantId > 0 && customerId > 0 && seats > 0) {
                Reservation reservation = new Reservation(restaurant, customer, table, seats, reservationDate);
                _mockRepoReservation.Setup(repo => repo.DoesReservationExist(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.DoesReservationOverlapCustomer(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.DoesReservationOverlapTable(reservation)).Returns(false);
                _mockRepoReservation.Setup(repo => repo.AddReservation(reservation)).Returns(reservation);

            }

            var result = _customerController.AddReservation(reservationRESTinput).Result as CreatedAtActionResult;
            Assert.IsType<ReservationRESToutputDTO>(result.Value);
            Assert.Equal(reservationDate, ((ReservationRESToutputDTO)result.Value).ReservationTime);
            Assert.Contains(restaurantId.ToString(), ((ReservationRESToutputDTO)result.Value).Restaurant.Id);
            Assert.Equal(seats, ((ReservationRESToutputDTO)result.Value).Seats);
            Assert.Contains(customerId.ToString(), ((ReservationRESToutputDTO)result.Value).Customer.CustomerID);
        }

        #endregion

        #region GetRestaurants
        [Theory]
        [InlineData("this is not a valid kitchen type", 9255)]
        [InlineData("french", 100)]
        [InlineData("italian", 23456456)]
        public void GetRestaurants_InvalidData_BadRequest(string? kitchen, int? postalcode) {
            _mockRepoConfig.Setup(repo => repo.GetKitchenTypes()).Returns(new List<string> { "french", "italian", "chinese" });

            var result = _customerController.GetRestaurants(kitchen, postalcode);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Theory]
        [InlineData("french", 9255)]
        [InlineData("chinese", null)]
        [InlineData(null, 9255)]
        [InlineData(null, null)]
        public void GetRestaurants_ValidData_OkResult(string? kitchen, int? postalcode) {
            _mockRepoConfig.Setup(repo => repo.GetKitchenTypes()).Returns(new List<string> { "french", "italian", "chinese" });

            List<Restaurant> restaurants = new List<Restaurant>();
            restaurants.Add(new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859"));
            restaurants.Add(new Restaurant("Not Cartoon", new Location(9280, "Lebbeke"), "italian", "info@cartoon.be", "+32478090859"));
            restaurants.Add(new Restaurant("resto 3", new Location(9255, "Lebbeke"), "italian", "info@cartoon.be", "+32478090859"));
            restaurants.Add(new Restaurant("resto 4", new Location(9255, "Lebbeke"), "french", "info@cartoon.be", "+32478090859"));
            restaurants[0].SetRestaurantId(3);
            restaurants[1].SetRestaurantId(4);
            restaurants[2].SetRestaurantId(5);
            restaurants[3].SetRestaurantId(6);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurants(kitchen, postalcode)).Returns(restaurants);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurants()).Returns(restaurants);
            if (kitchen != null) {
                _mockRepoConfig.Setup(repo => repo.ContainsKitchenType(kitchen)).Returns(true);
            }

            var result = _customerController.GetRestaurants(kitchen, postalcode);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Theory]
        [InlineData("french", 9255)]
        [InlineData("chinese", null)]
        [InlineData(null, 9255)]
        [InlineData(null, null)]
        public void GetRestaurants_ValidData_ListOfRestaurants(string? kitchen, int? postalcode) {
            _mockRepoConfig.Setup(repo => repo.GetKitchenTypes()).Returns(new List<string> { "french", "italian", "chinese" });

            List<Restaurant> restaurants = new List<Restaurant>();
            restaurants.Add(new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859"));
            restaurants.Add(new Restaurant("Not Cartoon", new Location(9280, "Lebbeke"), "italian", "info@cartoon.be", "+32478090859"));
            restaurants.Add(new Restaurant("resto 3", new Location(9255, "Lebbeke"), "italian", "info@cartoon.be", "+32478090859"));
            restaurants.Add(new Restaurant("resto 4", new Location(9255, "Lebbeke"), "french", "info@cartoon.be", "+32478090859"));
            restaurants[0].SetRestaurantId(3);
            restaurants[1].SetRestaurantId(4);
            restaurants[2].SetRestaurantId(5);
            restaurants[3].SetRestaurantId(6);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurants(kitchen, postalcode)).Returns(restaurants);
            _mockRepoRestaurant.Setup(repo => repo.GetRestaurants()).Returns(restaurants);
            if (kitchen != null) {
                _mockRepoConfig.Setup(repo => repo.ContainsKitchenType(kitchen)).Returns(true);
            }

            var result = _customerController.GetRestaurants(kitchen, postalcode).Result as OkObjectResult;
            // Gewoon een check of we een lijst hebben
            Assert.True(((List<RestaurantRESToutputDTO>)result.Value).Any(r => r.Name == "Cartoon"));
        }

        #endregion

        #region GetReservations
        [Theory]
        [InlineData(-1, null, null)]
        [InlineData(0, null, null)]
        [InlineData(1, 2050, 2020)]
        public void GetReservations_InvalidData_BadRequest(int customerId, int? startYear, int? endyear) {
            DateTime? day = startYear.HasValue ? new DateTime(startYear.Value, 01, 01) : null;
            DateTime? endTime = endyear.HasValue ? new DateTime(endyear.Value, 12, 31) : null;
            if (customerId > 0 && day.HasValue && endTime.HasValue) {
                Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
                customer.SetCustomerId(1);
                Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
                restaurant.SetRestaurantId(1);
                Reservation reservation = new Reservation(restaurant, customer, new Table(3, 4), 4, new DateTime(2030, 2, 2, 2, 30, 0));
                List<Reservation> reservations = new List<Reservation>();
                reservations.Add(reservation);
                if (day.HasValue && endTime.HasValue) {
                    _mockRepoReservation.Setup(repo => repo.GetReservationsOfCustomer(customerId, day.Value, endTime.Value)).Returns(reservations);
                } else if (endTime.HasValue) {
                    _mockRepoReservation.Setup(repo => repo.GetReservationsOfCustomer(customerId, new DateTime(1900, 01, 01), endTime.Value)).Returns(reservations);
                } else {
                    _mockRepoReservation.Setup(repo => repo.GetReservationsOfCustomer(customerId, day.Value, new DateTime(2999, 01, 01))).Returns(reservations);

                }
            }

            var result = _customerController.GetReservations(customerId, day, endTime);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Theory]
        [InlineData(1, null, null)]
        [InlineData(1, 2020, null)]
        [InlineData(1, 2020, 2050)]
        [InlineData(1, null, 2050)]
        public void GetReservations_ValidData_OkResult(int customerId, int? startYear, int? endyear) {
            DateTime? day = startYear.HasValue ? new DateTime(startYear.Value, 01, 01) : null;
            DateTime? endTime = endyear.HasValue ? new DateTime(endyear.Value, 12, 31) : null;
            if (customerId > 0) {
                Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
                customer.SetCustomerId(1);
                Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
                restaurant.SetRestaurantId(1);
                Reservation reservation = new Reservation(restaurant, customer, new Table(3, 4), 4, new DateTime(2030, 2, 2, 2, 30, 0));
                List<Reservation> reservations = new List<Reservation>();
                reservations.Add(reservation);
                if (day.HasValue && endTime.HasValue) {
                    _mockRepoReservation.Setup(repo => repo.GetReservationsOfCustomer(customerId, day.Value, endTime.Value)).Returns(reservations);
                } else if (endTime.HasValue) {
                    _mockRepoReservation.Setup(repo => repo.GetReservationsOfCustomer(customerId, new DateTime(1900, 01, 01), endTime.Value)).Returns(reservations);
                } else if (day.HasValue) {
                    _mockRepoReservation.Setup(repo => repo.GetReservationsOfCustomer(customerId, day.Value, new DateTime(2999, 12, 31))).Returns(reservations);
                } else {
                    _mockRepoReservation.Setup(repo => repo.GetReservationsOfCustomer(customerId, new DateTime(1900, 01, 01), new DateTime(2999, 12, 31))).Returns(reservations);
                }
            }

            var result = _customerController.GetReservations(customerId, day, endTime);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Theory]
        [InlineData(1, null, null)]
        [InlineData(1, 2020, null)]
        [InlineData(1, 2020, 2050)]
        [InlineData(1, null, 2050)]
        public void GetReservations_ValidData_ListOfRESTReservation(int customerId, int? startYear, int? endyear) {
            DateTime? day = startYear.HasValue ? new DateTime(startYear.Value, 01, 01) : null;
            DateTime? endTime = endyear.HasValue ? new DateTime(endyear.Value, 12, 31) : null;
            if (customerId > 0) {
                Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
                customer.SetCustomerId(1);
                Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
                restaurant.SetRestaurantId(1);
                Reservation reservation = new Reservation(restaurant, customer, new Table(3, 4), 4, new DateTime(2030, 2, 2, 2, 30, 0));
                List<Reservation> reservations = new List<Reservation>();
                reservations.Add(reservation);
                if (day.HasValue && endTime.HasValue) {
                    _mockRepoReservation.Setup(repo => repo.GetReservationsOfCustomer(customerId, day.Value, endTime.Value)).Returns(reservations);
                } else if (endTime.HasValue) {
                    _mockRepoReservation.Setup(repo => repo.GetReservationsOfCustomer(customerId, new DateTime(1900, 01, 01), endTime.Value)).Returns(reservations);
                } else if (day.HasValue) {
                    _mockRepoReservation.Setup(repo => repo.GetReservationsOfCustomer(customerId, day.Value, new DateTime(2999, 12, 31))).Returns(reservations);
                } else {
                    _mockRepoReservation.Setup(repo => repo.GetReservationsOfCustomer(customerId, new DateTime(1900, 01, 01), new DateTime(2999, 12, 31))).Returns(reservations);
                }
            }

            var result = _customerController.GetReservations(customerId, day, endTime);
            Assert.IsType<OkObjectResult>(result.Result);
        }

        DateTime dateHash0;
        [Fact]
        public void GetReservations_InvalidData_DateHashCode0_BadRequest() {
            var result1 = _customerController.GetReservations(1, dateHash0, null);
            var result2 = _customerController.GetReservations(1, dateHash0, dateHash0);
            var result3 = _customerController.GetReservations(1, null, dateHash0);
            Assert.IsType<BadRequestObjectResult>(result1.Result);
            Assert.IsType<BadRequestObjectResult>(result2.Result);
            Assert.IsType<BadRequestObjectResult>(result3.Result);
        }
        #endregion

        #region UpdateReservation

        [Theory]
        [InlineData(-1, 2025, 2)]
        [InlineData(0, 2025, null)]
        [InlineData(1, null, null)]
        [InlineData(1, 1999, null)]
        [InlineData(1, null, 0)]
        [InlineData(1, null, -1)]
        public void UpdateReservation_InvalidData_BadRequest(int reservationId, int? year, int? seats) {
            DateTime? day = year.HasValue ? new DateTime(year.Value, 01, 01) : null;

            if (reservationId > 0) {
                _mockRepoReservation.Setup(repo => repo.DoesReservationExist(reservationId)).Returns(true);

                Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
                customer.SetCustomerId(1);
                Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
                restaurant.SetRestaurantId(1);
                Reservation reservation = new Reservation(restaurant, customer, new Table(3, 4), 4, new DateTime(2030, 2, 2, 2, 30, 0));
                _mockRepoReservation.Setup(repo => repo.GetReservation(reservationId)).Returns(reservation);

                if (day.HasValue && day.Value > DateTime.Now && seats.HasValue && seats.Value > 0) {
                    _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(1, day.Value, seats.Value));
                    reservation.SetSeats(seats.Value);
                    reservation.SetDate(day.Value);
                } else if (day.HasValue && day.Value > DateTime.Now) {
                    _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(1, day.Value, reservation.Seats));
                    reservation.SetDate(day.Value);
                } else if (seats.HasValue && seats.Value > 0) {
                    _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(1, reservation.Date, seats.Value));
                    reservation.SetSeats(seats.Value);
                } else {
                    // both null should already give update
                }

                _mockRepoReservation.Setup(repo => repo.UpdateReservation(reservation)).Returns(reservation);
            } else {
                _mockRepoReservation.Setup(repo => repo.DoesReservationExist(reservationId)).Returns(false);

            }

            var result = _customerController.UpdateReservation(reservationId, day, seats);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public void UpdateReservation_UnknownReservationI_NotFound() {
            int reservationId = 3333;
            int? year = 2025;
            int? seats = 3;
            DateTime? day = year.HasValue ? new DateTime(year.Value, 01, 01) : null;

            if (reservationId > 0) {
                _mockRepoReservation.Setup(repo => repo.DoesReservationExist(reservationId)).Returns(false);

            } else {
                //_mockRepoReservation.Setup(repo => repo.DoesReservationExist(reservationId)).Returns(false);

            }

            var result = _customerController.UpdateReservation(reservationId, day, seats);
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }



        [Theory]
        [InlineData(1, 2030, null)]
        [InlineData(1, null, 4)]
        [InlineData(1, 2030, 4)]
        public void UpdateReservation_ValidData_NoUpdate_BadRequest(int reservationId, int? year, int? seats) {
            DateTime? day = year.HasValue ? new DateTime(year.Value, 01, 01) : null;

            if (reservationId > 0) {
                _mockRepoReservation.Setup(repo => repo.DoesReservationExist(reservationId)).Returns(true);

                Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
                customer.SetCustomerId(1);
                Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
                restaurant.SetRestaurantId(1);
                Reservation reservation = new Reservation(restaurant, customer, new Table(3, 4), 4, new DateTime(2030, 2, 2, 2, 30, 0)); 
                Reservation updatedReservation = new Reservation(reservationId, restaurant, customer, new Table(3, 4), 4, new DateTime(2030, 2, 2, 2, 30, 0), false);

                _mockRepoReservation.Setup(repo => repo.GetReservation(reservationId)).Returns(reservation);

                if (day.HasValue && day.Value > DateTime.Now && seats.HasValue && seats.Value > 0) {
                    _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(1, day.Value, seats.Value));
                    updatedReservation.SetSeats(seats.Value);
                    updatedReservation.SetDate(day.Value);
                } else if (day.HasValue && day.Value > DateTime.Now) {
                    _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(1, day.Value, reservation.Seats));
                    updatedReservation.SetDate(day.Value);
                } else if (seats.HasValue && seats.Value > 0) {
                    _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(1, reservation.Date, seats.Value));
                    updatedReservation.SetSeats(seats.Value);
                } else {
                    // both null should already give update
                }

                _mockRepoReservation.Setup(repo => repo.UpdateReservation(reservation)).Returns(updatedReservation);
            }

            var result = _customerController.UpdateReservation(reservationId, day, seats);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Theory]
        [InlineData(1, 2025, null)]
        [InlineData(1, null, 2)]
        [InlineData(1, 2025, 2)]
        public void UpdateReservation_ValidData_CreatedAtActionResult(int reservationId, int? year, int? seats) {
            DateTime? day = year.HasValue ? new DateTime(year.Value, 01, 01) : null;

            if (reservationId > 0) {
                _mockRepoReservation.Setup(repo => repo.DoesReservationExist(reservationId)).Returns(true);

                Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
                customer.SetCustomerId(1);
                Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
                restaurant.SetRestaurantId(1);
                Reservation reservation = new Reservation(reservationId, restaurant, customer, new Table(3, 8), 4, new DateTime(2030, 2, 2, 2, 30, 0), false);
                Reservation updatedReservation = new Reservation(reservationId, restaurant, customer, new Table(3, 4), 4, new DateTime(2030, 2, 2, 2, 30, 0), false);

                _mockRepoReservation.Setup(repo => repo.GetReservation(reservationId)).Returns(reservation);

                if (day.HasValue && day.Value > DateTime.Now && seats.HasValue && seats.Value > 0) {
                    _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(1, day.Value, seats.Value)).Returns(new Table(4,seats.Value));
                    updatedReservation.SetSeats(seats.Value);
                    updatedReservation.SetDate(day.Value);
                } else if (day.HasValue && day.Value > DateTime.Now) {
                    _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(1, day.Value, reservation.Seats)).Returns(new Table(4, 4));
                    updatedReservation.SetDate(day.Value);
                } else if (seats.HasValue && seats.Value > 0) {
                    _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(1, reservation.Date, seats.Value)).Returns(new Table(4, seats.Value));
                    updatedReservation.SetSeats(seats.Value);
                } else {
                    // both null should already give update
                }

                _mockRepoReservation.Setup(repo => repo.UpdateReservation(reservation)).Returns(updatedReservation);

            }

            var result = _customerController.UpdateReservation(reservationId, day, seats);
            Assert.IsType<CreatedAtActionResult>(result.Result);
        }

        [Theory]
        [InlineData(1, 2025, null)]
        [InlineData(1, null, 2)]
        [InlineData(1, 2025, 2)]
        public void UpdateReservation_ValidData_ReturnsReservation(int reservationId, int? year, int? seats) {
            DateTime? day = year.HasValue ? new DateTime(year.Value, 01, 01) : null;
            Reservation updatedReservation = null;
            if (reservationId > 0) {
                _mockRepoReservation.Setup(repo => repo.DoesReservationExist(reservationId)).Returns(true);

                Customer customer = new Customer("Mike", "info@mike.be", "+32478090859", new Location(9255, "Buggenhout"));
                customer.SetCustomerId(1);
                Restaurant restaurant = new Restaurant("Cartoon", new Location(9280, "Lebbeke"), "french", "info@cartoon.be", "+32478090859");
                restaurant.SetRestaurantId(1);
                Reservation reservation = new Reservation(reservationId, restaurant, customer, new Table(3, 8), 4, new DateTime(2030, 2, 2, 2, 30, 0), false);
                updatedReservation = new Reservation(reservationId, restaurant, customer, new Table(3, 4), 4, new DateTime(2030, 2, 2, 2, 30, 0), false);

                _mockRepoReservation.Setup(repo => repo.GetReservation(reservationId)).Returns(reservation);

                if (day.HasValue && day.Value > DateTime.Now && seats.HasValue && seats.Value > 0) {
                    _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(1, day.Value, seats.Value)).Returns(new Table(4, seats.Value));
                    updatedReservation.SetSeats(seats.Value);
                    updatedReservation.SetDate(day.Value);
                } else if (day.HasValue && day.Value > DateTime.Now) {
                    _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(1, day.Value, reservation.Seats)).Returns(new Table(4, 4));
                    updatedReservation.SetDate(day.Value);
                } else if (seats.HasValue && seats.Value > 0) {
                    _mockRepoReservation.Setup(repo => repo.ArrangeBestFitTableOrNull(1, reservation.Date, seats.Value)).Returns(new Table(4, seats.Value));
                    updatedReservation.SetSeats(seats.Value);
                } else {
                    // both null should already give update
                }

                _mockRepoReservation.Setup(repo => repo.UpdateReservation(reservation)).Returns(updatedReservation);

            }

            var result = _customerController.UpdateReservation(reservationId, day, seats).Result as CreatedAtActionResult;
            Assert.IsType<ReservationRESToutputDTO>(result.Value);
            Assert.Equal(updatedReservation.Seats, ((ReservationRESToutputDTO)result.Value).Seats);
            Assert.Equal(updatedReservation.Date, ((ReservationRESToutputDTO)result.Value).ReservationTime);
        }

        #endregion

        #region CancelReservation
        #endregion

        #region GetReservableRestaurants
        #endregion
    }
}
