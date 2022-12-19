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

        public CustomerController(CustomerService customerService) {
            _customerService = customerService;
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
                return Ok(_customerService.GetCustomer(customerId));
            } catch (Exception ex) {
                return BadRequest($"{nameof(GetCustomer)} - {ex.Message}");
            }
        }

        [HttpPost]
        public ActionResult<CustomerRESToutputDTO> AddCustomer([FromBody]CustomerRESTinputDTO customerRESTinput) {
            try {
                Customer customer = _customerService.AddCustomer(MapToDomain.MapCustomer(customerRESTinput));
                return CreatedAtAction(nameof(AddCustomer), new { customerId = customer.CustomerId }, MapToREST.MapCustomer(hostURL, customer));
            } catch (Exception ex) {
                return BadRequest($"{nameof(AddCustomer)} - {ex.Message}");
            }
        }

        #endregion
    }
}
