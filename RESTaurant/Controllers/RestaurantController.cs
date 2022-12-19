using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTaurant.Mappers;
using RESTaurant.Model.Input;
using RESTaurant.Model.Output;
using RESTaurantBL.Model;
using RESTaurantBL.Services;
using RESTaurantDLEF.EFModel;

namespace RESTaurant.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase {
        private string hostURL = "http://localhost:5298/api/Restaurant";
        private RestaurantService restaurantService;

        public RestaurantController(RestaurantService restaurantService) {
            this.restaurantService = restaurantService;
        }

        #region RestaurantInfo
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

        [HttpPut("{restaurantId}")]
        public IActionResult UpdateRestaurant(int restaurantId, [FromBody] RestaurantRESTinputDTO restaurantRESTinput) {
            try {
                if (restaurantService.DoesExist(restaurantId)) {
                    Restaurant r = MapToDomain.MapRestaurant(restaurantId, restaurantRESTinput);
                    r = restaurantService.UpdateRestaurant(r);
                    return CreatedAtAction(nameof(UpdateRestaurant), restaurantId, MapToREST.MapRestaurant(hostURL, r));
                } else {
                    return NotFound("Restaurant niet gevonden");
                }
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{restaurantId}")]
        public IActionResult DeleteRestaurant(int restaurantId) {
            try {
                if (restaurantService.DoesExist(restaurantId)) {
                    restaurantService.DeleteRestaurant(restaurantId);
                    return NoContent();
                } else {
                    return NotFound("Restaurant niet gevonden");
                }
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        #endregion
        #region RestaurantDetails
        [HttpGet("{restaurantId}/Details")]
        public ActionResult<RestaurantDetailRESToutputDTO> GetRestaurantDetails(int restaurantId) {
            try {
                if (restaurantId <= 0) { return BadRequest("Invalid id"); }
                return Ok(MapToREST.MapRestaurantDetails(hostURL, restaurantId, restaurantService));
            } catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }
        #endregion
        #region Tables
        [HttpPost]
        [Route("{restaurantId}/Table")]
        public IActionResult AddRestaurantTable(int restaurantId, [FromBody] RestaurantTableRESTinputDTO tableRESTinput) {
            try {
                restaurantService.AddTableToRestaurant(restaurantId, tableRESTinput.TableNumber, tableRESTinput.Seats);
                return CreatedAtAction(nameof(GetRestaurantDetails), new { restaurantId = restaurantId }, MapToREST.MapRestaurantDetails(hostURL, restaurantId, restaurantService));
            } catch (Exception ex) {
                return NotFound(ex.Message);
            }
        }

        [HttpPut]
        [Route("{restaurantId}/Table")]
        public IActionResult UpdateTableOfRestaurant(int restaurantId, [FromBody] RestaurantTableRESTinputDTO tableRESTinput) {
            try {
                if (restaurantService.DoesExist(restaurantId)) {
                    restaurantService.UpdateTableOfRestaurant(restaurantId, tableRESTinput.TableNumber, tableRESTinput.Seats);
                    return CreatedAtAction(nameof(UpdateTableOfRestaurant), new { restaurantId = restaurantId }, MapToREST.MapRestaurantDetails(hostURL, restaurantId, restaurantService));
                } else {
                    return NotFound("Restaurant niet gevonden");
                }
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{restaurantId}/Table/{tablenumber}")]
        public IActionResult DeleteTableRestaurant(int restaurantId, int tablenumber) {
            try {
                if (restaurantId <= 0) { return BadRequest("Invalid id"); }
                restaurantService.DeleteTableOfRestaurant(restaurantId, tablenumber);
                return NoContent();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }
        #endregion
    }
}
