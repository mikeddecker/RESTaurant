using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTaurant.Mappers;
using RESTaurant.Model.Input;
using RESTaurant.Model.Output;
using RESTaurant_BL.Model;
using RESTaurant_BL.Services;
using RESTaurant_DL.EFModel;

namespace RESTaurant.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase {
        private string hostURL = "http://localhost:5298/api/Restaurant";
        private RestaurantService restaurantService;

        public RestaurantController(RestaurantService restaurantService) {
            this.restaurantService = restaurantService;
        }

        [HttpGet]
        public ActionResult<List<RestaurantRESToutputDTO>> GetRestaurants() {
            try {
                List<Restaurant> restaurants = restaurantService.GetRestaurants();
                List<RestaurantRESToutputDTO> restaurantListRESToutputs = MapToREST.MapToListFromDomain(hostURL, restaurants);
                return Ok(restaurantListRESToutputs);
            } catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{restaurantId}")]
        public ActionResult<RestaurantRESToutputDTO> GetRestaurant(int restaurantId) {
            try {
                Restaurant restaurant = restaurantService.GetRestaurant(restaurantId);
                return Ok(MapToREST.MapRestaurant(hostURL, restaurant));
            } catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public ActionResult<RestaurantRESToutputDTO> AddRestaurant([FromBody] RestaurantRESTinputDTO restaurantRESTinput) {
            try {
                Restaurant restaurant = restaurantService.AddRestaurant(MapToDomain.MapRestaurant(restaurantRESTinput));
                return CreatedAtAction(nameof(GetRestaurant), new { restaurantId = restaurant.RestaurantId }, MapToREST.MapRestaurant(hostURL, restaurant));
            } catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }
    }
}
