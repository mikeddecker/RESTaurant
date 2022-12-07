using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RESTaurant_BL.Services;

namespace RESTaurant.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class RestaurantController : ControllerBase {
        private string hostURL = "http://localhost:5298";
        private RestaurantService restaurantService;

        public RestaurantController(RestaurantService restaurantService) {
            this.restaurantService = restaurantService;
        }
    }
}
