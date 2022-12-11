using RESTaurant_BL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant_BL.Interfaces {
    public interface IRestaurantRepository
    {
        Restaurant AddRestaurant(Restaurant restaurant);
        bool DoesExist(Restaurant restaurant);
        bool DoesExist(int restaurantId);
        List<Restaurant> GetRestaurants();
        Restaurant GetRestaurant(int restaurantId);
    }
}
