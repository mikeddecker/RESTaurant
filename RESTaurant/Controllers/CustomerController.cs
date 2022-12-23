using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTaurant.Model.Input;
using RESTaurant.Model.Output;
using RESTaurant.Mappers;
using RESTaurantBL.Model;
using RESTaurantBL.Services;
using RESTaurantDLEF.EFModel;

namespace RESTaurant.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase {
        private string hostURL = "http://localhost:5298/api/Customer"; // TODO update url --> GetRestaurant "id": "http://localhost:5298/api/Customer/1", "name": "Cartoon",
        private CustomerService _customerService;
        private ReservationService _reservationService;
        private RestaurantService _restaurantService;

        public CustomerController(CustomerService customerService, RestaurantService restaurantService, ReservationService reservationService) {
            _customerService = customerService;
            _restaurantService = restaurantService;
            _reservationService = reservationService;
        }

        #region Customer
        [HttpGet]
        public ActionResult<List<CustomerRESToutputDTO>> GetCustomers() {
            try {
                List<Customer> customers = _customerService.GetCustomers();
                List<CustomerRESToutputDTO> customersDTO = MapToREST.MapCustomerList(hostURL, customers);
                return Ok(customersDTO);
            } catch (Exception ex) {
                return NotFound($"{nameof(GetCustomers)} - {ex.Message}");
            }
        }

        [HttpGet("{customerId}")]
        public ActionResult<CustomerRESToutputDTO> GetCustomer(int customerId) {
            try {
                if (customerId <= 0) { return BadRequest($"{nameof(GetCustomer)} - Invalid CustomerId"); }
                if (!_customerService.DoesCustomerExist(customerId)) { return NotFound($"{nameof(GetCustomer)} - Customer does not exist"); }
                return Ok(MapToREST.MapCustomer(hostURL, _customerService.GetCustomer(customerId)));
            } catch (Exception ex) {
                return BadRequest($"{nameof(GetCustomer)} - {ex.Message}");
            }
        }

        [HttpPost]
        public ActionResult<CustomerRESToutputDTO> AddCustomer([FromBody] CustomerRESTinputDTO customerRESTinput) {
            try {
                Customer customer = _customerService.AddCustomer(MapToDomain.MapCustomer(customerRESTinput));
                return CreatedAtAction(nameof(AddCustomer), new { customerId = customer.CustomerId }, MapToREST.MapCustomer(hostURL, customer));
            } catch (Exception ex) {
                return BadRequest($"{nameof(AddCustomer)} - {ex.Message}");
            }
        }

        [HttpPut("{customerId}")]
        public ActionResult<CustomerRESToutputDTO> UpdateCustomer(int customerId, [FromBody] CustomerRESTinputDTO customerRESTinput) {
            try {
                Customer customer = _customerService.UpdateCustomer(MapToDomain.MapCustomer(customerId, customerRESTinput));
                return CreatedAtAction(nameof(UpdateCustomer), new { customerId = customer.CustomerId }, MapToREST.MapCustomer(hostURL, customer));
            } catch (Exception ex) {
                return BadRequest($"{nameof(UpdateCustomer)} - {ex.Message}");
            }
        }

        [HttpDelete("Goodbye/{customerId}")]
        public IActionResult DeleteCustomer(int customerId) {
            try {
                if (customerId <= 0) { return BadRequest($"{nameof(DeleteCustomer)} - Invalid customerId"); }
                _customerService.DeleteCustomer(customerId);
                return NoContent();
            } catch (Exception ex) {
                return BadRequest($"{nameof(UpdateCustomer)} - {ex.Message}");
            }
        }
        #endregion
        #region Reservation
        [HttpPost]
        [Route("Reservation")]
        public ActionResult<ReservationRESTinputDTO> AddReservation([FromBody] ReservationRESTinputDTO reservationRESTinput) {
            try {
                (bool, int) reservableTablenumber = _reservationService.CanMakeReservation_GetTablenumber(reservationRESTinput.RestaurantId, reservationRESTinput.Date, reservationRESTinput.Seats);
                if (!reservableTablenumber.Item1) {
                    Restaurant restaurant = _restaurantService.GetRestaurant(reservationRESTinput.RestaurantId);
                    return BadRequest($"Can't make a reservation on {reservationRESTinput.Date} for {reservationRESTinput.Seats} at {restaurant.Name} ");
                }
                int tableNumber = reservableTablenumber.Item2; //_reservationService.GetTableForReservation(reservationRESTinput.RestaurantId, reservationRESTinput.Date, reservationRESTinput.Seats);
                Reservation reservation = _reservationService.AddReservation(MapToDomain.MapReservation(reservationRESTinput, tableNumber, _customerService, _restaurantService));
                return CreatedAtAction(nameof(AddReservation), new { ReservationId = reservation.ReservationId }, MapToREST.MapReservation(hostURL, reservation));
            } catch (Exception ex) {
                return BadRequest($"{nameof(AddCustomer)} - {ex.Message}");
            }
        }

        [HttpGet]
        [Route("Restaurant")]
        public ActionResult<List<RestaurantRESToutputDTO>> GetRestaurants([FromQuery] string? kitchen, [FromQuery] int? postalCode) {
            try {
                if (!string.IsNullOrWhiteSpace(kitchen) && !_restaurantService.ContainsKitchenType(kitchen)) { return BadRequest($"Invalid kitchentype {kitchen}"); }
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
                return BadRequest($"{nameof(GetRestaurants)} - {ex.Message}");
            }
        }

        [HttpGet]
        [Route("ReservableRestaurants")]
        public ActionResult<List<ReservationRESToutputDTO>> GetReservableRestaurants([FromQuery] DateTime date) {
            try {
                if (date.GetHashCode() == 0) { return BadRequest($"Date hashcode 0"); }
                if (date < DateTime.Now) { return BadRequest($"Date can't be in the past"); }
                return Ok(MapToREST.MapRestaurantList(hostURL, _reservationService.CanIMakeReservation(date)));
            } catch (Exception ex) {
                return BadRequest($"{nameof(GetReservableRestaurants)} - {ex.Message}");
            }
        }

        [HttpGet]
        [Route("Reservation/{customerId}")]
        public ActionResult<List<ReservationRESToutputDTO>> GetReservations(int customerId, [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime) {
            try {
                DateTime beginDate = new DateTime(1900, 01, 01);
                DateTime endDate = new DateTime(2999, 12, 31);
                if (startTime.GetHashCode() != 0) { beginDate = startTime.Value; }
                if (endTime.GetHashCode() != 0) { endDate = endTime.Value; }
                //if (date < DateTime.Now) { return BadRequest($"Date can't be in the past"); }
                return Ok(MapToREST.MapReservationList(hostURL, _reservationService.GetReservationsOfCustomer(customerId, beginDate, endDate)));
            } catch (Exception ex) {
                return BadRequest($"{nameof(GetReservations)} - {ex.Message}");
            }
        }

        [HttpPut]
        [Route("CancelReservation/{reservationId}")]
        public IActionResult CancelReservation(int reservationId) {
            try {
                _reservationService.CancelReservation(reservationId);
                return NoContent();
            } catch (Exception ex) {
                return BadRequest($"{nameof(CancellationToken)} - {ex.Message}");
            }
        }

        [HttpPut]
        [Route("Reservation/{reservationId}")]
        public IActionResult UpdateReservation(int reservationId, [FromQuery] DateTime? date, [FromQuery] int? seats) {
            try {
                // At least one shoulde be filled in
                if (!date.HasValue && !seats.HasValue) { return BadRequest($"{nameof(UpdateReservation)} - No update"); }

                if (_reservationService.DoesReservationExist(reservationId)) {
                    Reservation reservation = _reservationService.UpdateReservation(reservationId, date, seats);
                    return CreatedAtAction(nameof(UpdateReservation), reservationId, MapToREST.MapReservation(hostURL, reservation));
                } else {
                    return NotFound($"Reservation {reservationId} not found");
                }
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}
