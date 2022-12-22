using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTaurant.Model.Input;
using RESTaurant.Model.Output;
using RESTaurant.Mappers;
using RESTaurantBL.Model;
using RESTaurantBL.Services;

namespace RESTaurant.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase {
        private string hostURL = "http://localhost:5298/api/Customer";
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
                return Ok(MapToREST.MapCustomer(hostURL,_customerService.GetCustomer(customerId)));
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
        public ActionResult<CustomerRESToutputDTO> DeleteCustomer(int customerId) {
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
        public ActionResult<ReservationRESTinputDTO> AddReservation([FromBody]ReservationRESTinputDTO reservationRESTinput) {
            try {
                Reservation reservation = _reservationService.AddReservation(MapToDomain.MapReservation(reservationRESTinput, _customerService, _restaurantService));
                return CreatedAtAction(nameof(AddReservation), new { ReservationId = reservation.ReservationId }, MapToREST.MapReservation(hostURL, reservation));
            } catch (Exception ex) {
                return BadRequest($"{nameof(AddCustomer)} - {ex.Message}");
            }
        }
        #endregion
    }
}
