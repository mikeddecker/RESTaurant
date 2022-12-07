using RESTaurant_BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant_BL.Services {
    public class RestaurantService {
        private IRestaurantRepository restaurantRepo;

        public RestaurantService(IRestaurantRepository restaurantRepo) {
            this.restaurantRepo = restaurantRepo;
        }

        public static List<string> GetKitchenTypes() {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            return new List<string>(ConfigurationManager.AppSettings["kitchenTypes"].Split(';'));
#pragma warning restore CS8602 // Dereference of a possibly null reference.
        }
    }
}
