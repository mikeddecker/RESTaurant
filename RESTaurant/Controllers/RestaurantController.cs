using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTaurant.Mappers;
using RESTaurant.Model.Input;
using RESTaurant.Model.Output;
using RESTaurantBL.Model;
using RESTaurantBL.Services;

namespace RESTaurant.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase {
        private string hostURL = "http://localhost:5298/api";
        private RestaurantService _restaurantService;
        private ReservationService _reservationService;
        private ILogger _logger;

        public RestaurantController(RestaurantService restaurantService, ReservationService reservationService, ILoggerFactory loggerFactory) {
            _restaurantService = restaurantService;
            _reservationService = reservationService;
            _logger = loggerFactory.AddFile("RestaurantLogs.txt").CreateLogger("RestaurantLogger");
        }

        #region Restaurant
        [HttpPost]
        public ActionResult<RestaurantRESToutputDTO> AddRestaurant([FromBody] RestaurantRESTinputDTO restaurantRESTinput) {
            try {
                _logger.LogInformation($"{nameof(AddRestaurant)}, {restaurantRESTinput}");
                Restaurant restaurant = _restaurantService.AddRestaurant(MapToDomain.MapRestaurant(restaurantRESTinput));
                return CreatedAtAction(nameof(GetRestaurant), new { restaurantId = restaurant.RestaurantId }, MapToREST.MapRestaurant(hostURL, restaurant));
            } catch (Exception ex) {
                _logger.LogError($"{nameof(AddRestaurant)} - {ex.Message}");
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public ActionResult<List<RestaurantRESToutputDTO>> GetRestaurants() {
            try {
                _logger.LogInformation(nameof(GetRestaurants));
                List<Restaurant> restaurants = _restaurantService.GetRestaurants();
                List<RestaurantRESToutputDTO> restaurantListRESToutputs = MapToREST.MapRestaurantList(hostURL, restaurants);
                return Ok(restaurantListRESToutputs);
            } catch (Exception ex) {
                _logger.LogError($"{nameof(GetRestaurants)} - {ex.Message}");
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{restaurantId}")]
        public ActionResult<RestaurantRESToutputDTO> GetRestaurant(int restaurantId) {
            try {
                _logger.LogInformation($"{nameof(GetRestaurant)}, {restaurantId}");
                Restaurant restaurant = _restaurantService.GetRestaurant(restaurantId);
                return Ok(MapToREST.MapRestaurant(hostURL, restaurant));
            } catch (Exception ex) {
                _logger.LogError($"{nameof(GetRestaurant)} - {ex.Message}");
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{restaurantId}/Details")]
        public ActionResult<RestaurantDetailRESToutputDTO> GetRestaurantDetails(int restaurantId) {
            try {
                _logger.LogInformation($"{nameof(GetRestaurantDetails)}, {restaurantId}");
                if (restaurantId <= 0) { return BadRequest("Invalid id"); }
                return Ok(MapToREST.MapRestaurantDetails(hostURL, restaurantId, _restaurantService));
            } catch (Exception ex) {
                _logger.LogError($"{nameof(GetRestaurantDetails)} - {ex.Message}");
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{restaurantId}")]
        public IActionResult UpdateRestaurant(int restaurantId, [FromBody] RestaurantRESTinputDTO restaurantRESTinput) {
            try {
                _logger.LogInformation($"{nameof(UpdateRestaurant)}, {restaurantId}");
                Restaurant r = MapToDomain.MapRestaurant(restaurantId, restaurantRESTinput);
                r = _restaurantService.UpdateRestaurant(r);
                return CreatedAtAction(nameof(UpdateRestaurant), restaurantId, MapToREST.MapRestaurant(hostURL, r));
            } catch (Exception ex) {
                _logger.LogError($"{nameof(UpdateRestaurant)} - {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{restaurantId}")]
        public IActionResult DeleteRestaurant(int restaurantId) {
            try {
                _logger.LogInformation($"{nameof(DeleteRestaurant)}, {restaurantId}");
                _restaurantService.DeleteRestaurant(restaurantId);
                return NoContent();
            } catch (Exception ex) {
                _logger.LogError($"{nameof(DeleteRestaurant)} - {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Tables
        [HttpPost]
        [Route("{restaurantId}/Table")]
        public IActionResult AddRestaurantTable(int restaurantId, [FromBody] RestaurantTableRESTinputDTO tableRESTinput) {
            try {
                _logger.LogInformation($"{nameof(AddRestaurantTable)}, {restaurantId}, {tableRESTinput}");
                _restaurantService.AddTable(restaurantId, tableRESTinput.TableNumber, tableRESTinput.Seats);
                return CreatedAtAction(nameof(GetRestaurantDetails), new { restaurantId = restaurantId }, MapToREST.MapRestaurantDetails(hostURL, restaurantId, _restaurantService));
            } catch (Exception ex) {
                _logger.LogError($"{nameof(AddRestaurantTable)} - {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        // Get is with GetDetails of restaurant

        [HttpPut]
        [Route("{restaurantId}/Table")]
        public IActionResult UpdateTableOfRestaurant(int restaurantId, [FromBody] RestaurantTableRESTinputDTO tableRESTinput) {
            try {
                _logger.LogInformation($"{nameof(UpdateTableOfRestaurant)}, {restaurantId}, {tableRESTinput}");
                _restaurantService.UpdateTable(restaurantId, tableRESTinput.TableNumber, tableRESTinput.Seats);
                return CreatedAtAction(nameof(UpdateTableOfRestaurant), new { restaurantId = restaurantId }, MapToREST.MapRestaurantDetails(hostURL, restaurantId, _restaurantService));
            } catch (Exception ex) {
                _logger.LogError($"{nameof(UpdateTableOfRestaurant)} - {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{restaurantId}/Table/{tablenumber}")]
        public IActionResult DeleteTableRestaurant(int restaurantId, int tablenumber) {
            try {
                _logger.LogInformation($"{nameof(DeleteTableRestaurant)}, {restaurantId}, {tablenumber}");
                if (restaurantId <= 0) { return BadRequest("Invalid id"); }
                _restaurantService.DeleteTable(restaurantId, tablenumber);
                return NoContent();
            } catch (Exception ex) {
                _logger.LogError($"{nameof(DeleteTableRestaurant)} - {ex.Message}");
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region RestaurantReservations

        [HttpGet("{restaurantId}/Reservations")]
        public ActionResult<List<ReservationRESToutputDTO>> GetReservations(int restaurantId, [FromQuery] DateTime? day, [FromQuery] DateTime? endDate) {
            try {
                _logger.LogInformation($"{nameof(DeleteTableRestaurant)}, {restaurantId}, {day}, {endDate}");
                List<Reservation> reservations = _reservationService.GetReservations(restaurantId, day, endDate);
                List<ReservationRESToutputDTO> reservationListRESToutputs = MapToREST.MapReservationList(hostURL, reservations);
                return Ok(reservationListRESToutputs);
            } catch (Exception ex) {
                _logger.LogError($"{nameof(GetReservations)} - {ex.Message}");
                return NotFound(ex.Message);
            }
        }

        #endregion
    }
}
