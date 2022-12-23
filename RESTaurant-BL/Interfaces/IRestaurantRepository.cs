using RESTaurantBL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantBL.Interfaces {
    public interface IRestaurantRepository {
        Restaurant AddRestaurant(Restaurant restaurant);
        bool DoesRestaurantExist(Restaurant restaurant);
        bool DoesRestaurantExist(int restaurantId);
        List<Restaurant> GetRestaurants();
        Restaurant GetRestaurant(int restaurantId);
        Restaurant UpdateRestaurant(Restaurant restaurant);
        void DeleteRestaurant(int restaurantId);
        bool HasRestaurantTableNumber(int restaurantId, int tableNumber);
        void AddTableToRestaurant(int restaurantId, int tableNumber, int seats);
        Dictionary<int, int> GetTablesOfRestaurant(int restaurantId);
        void DeleteTableOfRestaurant(int restaurantId, int tablenumber);
        void UpdateTableOfRestaurant(int restaurantId, int tableNumber, int seats);
        Table GetTableOfRestaurant(int restaurantId, int tableNumber);
        List<Restaurant> GetRestaurants(string kitchen, int? postalCode);
    }
}
