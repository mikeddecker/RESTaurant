using RESTaurantBL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantBL.Interfaces {
    public interface IRestaurantRepository
    {
        Restaurant AddRestaurant(Restaurant restaurant);
        bool DoesExist(Restaurant restaurant);
        bool DoesExist(int restaurantId);
        List<Restaurant> GetRestaurants();
        Restaurant GetRestaurant(int restaurantId);
        Restaurant UpdateRestaurant(Restaurant restaurant);
        void DeleteRestaurant(int restaurantId);
    }
}
