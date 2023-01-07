using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTaurant.Model.Input;
using RESTaurant.Model.Output;
using RESTaurant.Mappers;
using RESTaurantBL.Model;
using RESTaurantBL.Services;
using RESTaurantDLEF.EFModel;
using System.Reflection.Metadata.Ecma335;
using RESTaurant.Exceptions;
using Microsoft.Extensions.Logging;

namespace RESTaurant.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase {
        private string hostURL = "http://localhost:5298/api"; // TODO update url --> GetRestaurant "id": "http://localhost:5298/api/Customer/1", "name": "Cartoon",
        private CustomerService _customerService;
        private ReservationService _reservationService;
        private RestaurantService _restaurantService;
        private ILogger _logger;

        public CustomerController(CustomerService customerService, RestaurantService restaurantService, ReservationService reservationService, ILoggerFactory loggerFactory) {
            _customerService = customerService;
            _restaurantService = restaurantService;
            _reservationService = reservationService;
            _logger = loggerFactory.AddFile("CustomerLogs.txt").CreateLogger("CustomerLogger");
        }

        #region Customer
        [HttpPost]
        public ActionResult<CustomerRESToutputDTO> AddCustomer([FromBody] CustomerRESTinputDTO customerRESTinput) {
            try {
                _logger.LogInformation($"{nameof(AddCustomer)}, {customerRESTinput}");
                Customer customer = _customerService.AddCustomer(MapToDomain.MapCustomer(customerRESTinput));
                return CreatedAtAction(nameof(AddCustomer), new { customerId = customer.CustomerId }, MapToREST.MapCustomer(hostURL, customer));
            } catch (Exception ex) {
                _logger.LogError($"{nameof(AddCustomer)} - {ex.Message}");
                return BadRequest($"{nameof(AddCustomer)} - {ex.Message}");
            }
        }

        [HttpGet]
        public ActionResult<List<CustomerRESToutputDTO>> GetCustomers() {
            try {
                _logger.LogInformation($"{nameof(GetCustomers)}");
                List<Customer> customers = _customerService.GetCustomers();
                List<CustomerRESToutputDTO> customersDTO = MapToREST.MapCustomerList(hostURL, customers);
                return Ok(customersDTO);
            } catch (Exception ex) {
                _logger.LogError($"{nameof(GetCustomers)} - {ex.Message}");
                return NotFound($"{nameof(GetCustomers)} - {ex.Message}");
            }
        }

        [HttpGet("{customerId}")]
        public ActionResult<CustomerRESToutputDTO> GetCustomer(int customerId) {
            try {
                _logger.LogInformation($"{nameof(GetCustomer)}, {customerId}");
                if (customerId <= 0) { return BadRequest($"{nameof(GetCustomer)} - Invalid CustomerId"); }
                if (!_customerService.DoesCustomerExist(customerId)) { return NotFound($"{nameof(GetCustomer)} - Customer does not exist"); }
                return Ok(MapToREST.MapCustomer(hostURL, _customerService.GetCustomer(customerId)));
            } catch (Exception ex) {
                _logger.LogError($"{nameof(GetCustomer)} - {ex.Message}");
                return BadRequest($"{nameof(GetCustomer)} - {ex.Message}");
            }
        }

        [HttpPut("{customerId}")]
        public ActionResult<CustomerRESToutputDTO> UpdateCustomer(int customerId, [FromBody] CustomerRESTinputDTO customerRESTinput) {
            try {
                _logger.LogInformation($"{nameof(UpdateCustomer)}, {customerId}, {customerRESTinput}");
                Customer customer = _customerService.UpdateCustomer(MapToDomain.MapCustomer(customerId, customerRESTinput));
                return CreatedAtAction(nameof(UpdateCustomer), new { customerId = customer.CustomerId }, MapToREST.MapCustomer(hostURL, customer));
            } catch (Exception ex) {
                _logger.LogError($"{nameof(UpdateCustomer)} - {ex.Message}");
                return BadRequest($"{nameof(UpdateCustomer)} - {ex.Message}");
            }
        }

        [HttpDelete("Goodbye/{customerId}")]
        public IActionResult DeleteCustomer(int customerId) {
            try {
                _logger.LogInformation($"{nameof(DeleteCustomer)}, {customerId}");
                if (customerId <= 0) { return BadRequest($"{nameof(DeleteCustomer)} - Invalid customerId"); }
                _customerService.DeleteCustomer(customerId);
                return NoContent();
            } catch (Exception ex) {
                _logger.LogError($"{nameof(DeleteCustomer)} - {ex.Message}");
                return BadRequest($"{nameof(DeleteCustomer)} - {ex.Message}");
            }
        }
        #endregion

        #region Reservation
        [HttpPost]
        [Route("Reservation")]
        public ActionResult<ReservationRESToutputDTO> AddReservation([FromBody] ReservationRESTinputDTO reservationRESTinput) {
            try {
                _logger.LogInformation($"{nameof(AddReservation)}, {reservationRESTinput}");
                Table reservableTable = _reservationService.ArrangeTableNumberOrNull(reservationRESTinput.RestaurantId, reservationRESTinput.Date, reservationRESTinput.Seats) ?? throw new CustomerControllerException($"Can't make a reservation on {reservationRESTinput.Date} for {reservationRESTinput.Seats} at {_restaurantService.GetRestaurant(reservationRESTinput.RestaurantId).Name}");

                Reservation reservation = _reservationService.AddReservation(MapToDomain.MapReservation(reservationRESTinput, reservableTable.TableNumber, _customerService, _restaurantService));
                return CreatedAtAction(nameof(AddReservation), new { ReservationId = reservation.ReservationId }, MapToREST.MapReservation(hostURL, reservation));
            } catch (CustomerControllerException ex) {
                _logger.LogError($"{nameof(AddReservation)} - {ex.Message}");
                return NotFound($"{nameof(AddCustomer)} - {ex.Message}");
            } catch (Exception ex) {
                _logger.LogError($"{nameof(AddReservation)} - {ex.Message}");
                return BadRequest($"{nameof(AddReservation)} - {ex.Message}");
            }
        }

        [HttpGet]
        [Route("Restaurant")]
        public ActionResult<List<RestaurantRESToutputDTO>> GetRestaurants([FromQuery] string? kitchen, [FromQuery] int? postalCode) {
            try {
                _logger.LogInformation($"{nameof(GetRestaurants)}, {kitchen}, {postalCode}");
                if (!string.IsNullOrWhiteSpace(kitchen) && !_restaurantService.ContainsKitchenType(kitchen)) {
                    string message = $"Invalid kitchentype {kitchen}";
                    _logger.LogError($"{nameof(GetRestaurants)} - {message}");
                    return BadRequest(message);
                }
                if (postalCode.HasValue) {
                    if (postalCode.Value > 9999 || postalCode.Value < 1000) { return BadRequest($"Invalid postal code {postalCode}"); }
                } else if (!string.IsNullOrWhiteSpace(kitchen)) {

                }
                if (string.IsNullOrWhiteSpace(kitchen) && !postalCode.HasValue) {
                    return Ok(MapToREST.MapRestaurantList(hostURL, _restaurantService.GetRestaurants()));
                } else {
                    return Ok(MapToREST.MapRestaurantList(hostURL, _restaurantService.GetRestaurants(kitchen, postalCode)));
                }
            } catch (Exception ex) {
                _logger.LogError($"{nameof(GetRestaurants)} - {ex.Message}");
                return BadRequest($"{nameof(GetRestaurants)} - {ex.Message}");
            }
        }

        [HttpGet]
        [Route("Reservation/{customerId}")]
        public ActionResult<List<ReservationRESToutputDTO>> GetReservations(int customerId, [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime) {
            try {
                _logger.LogInformation($"{nameof(GetReservations)}, {customerId}, {startTime}, {endTime}");
                DateTime beginDate = new DateTime(1900, 01, 01);
                DateTime endDate = new DateTime(2999, 12, 31);
                if (startTime.GetHashCode() != 0) { beginDate = startTime.Value; }
                if (endTime.GetHashCode() != 0) { endDate = endTime.Value; }
                //if (date < DateTime.Now) { return BadRequest($"Date can't be in the past"); }
                return Ok(MapToREST.MapReservationList(hostURL, _reservationService.GetReservationsOfCustomer(customerId, beginDate, endDate)));
            } catch (Exception ex) {
                _logger.LogError($"{nameof(GetReservations)} - {ex.Message}");
                return BadRequest($"{nameof(GetReservations)} - {ex.Message}");
            }
        }

        [HttpPut]
        [Route("Reservation/{reservationId}")]
        public IActionResult UpdateReservation(int reservationId, [FromQuery] DateTime? date, [FromQuery] int? seats) {
            try {
                _logger.LogInformation($"{nameof(UpdateReservation)}, {reservationId}, {date}, {seats}");
                // At least one shoulde be filled in
                if (!date.HasValue && !seats.HasValue) { return BadRequest($"{nameof(UpdateReservation)} - No update"); }

                if (_reservationService.DoesReservationExist(reservationId)) {
                    Reservation reservation = _reservationService.UpdateReservation(reservationId, date, seats);
                    return CreatedAtAction(nameof(UpdateReservation), reservationId, MapToREST.MapReservation(hostURL, reservation));
                } else {
                    return NotFound($"Reservation {reservationId} not found");
                }
            } catch (Exception ex) {
                _logger.LogError($"{nameof(UpdateReservation)} - {ex.Message}");
                return BadRequest($"{nameof(UpdateReservation)} - {ex.Message}");
            }
        }

        [HttpPut]
        [Route("CancelReservation/{reservationId}")]
        public IActionResult CancelReservation(int reservationId) {
            try {
                _logger.LogInformation($"{nameof(CancelReservation)}, {reservationId}");
                _reservationService.CancelReservation(reservationId);
                return NoContent();
            } catch (Exception ex) {
                _logger.LogError($"{nameof(CancelReservation)} - {ex.Message}");
                return BadRequest($"{nameof(CancelReservation)} - {ex.Message}");
            }
        }

        #endregion


        [HttpGet]
        [Route("ReservableRestaurants")]
        public ActionResult<List<ReservationRESToutputDTO>> GetReservableRestaurants([FromQuery] DateTime date) {
            try {
                _logger.LogInformation($"{nameof(GetReservableRestaurants)}, {date}");
                if (date.GetHashCode() == 0) { return BadRequest($"Date hashcode 0"); }
                if (date < DateTime.Now) { return BadRequest($"Date can't be in the past"); }
                return Ok(MapToREST.MapRestaurantList(hostURL, _reservationService.GetReservableRestaurantsOnDate(date)));
            } catch (Exception ex) {
                _logger.LogError($"{nameof(GetReservableRestaurants)} - {ex.Message}");
                return BadRequest($"{nameof(GetReservableRestaurants)} - {ex.Message}");
            }
        }
    }
}
