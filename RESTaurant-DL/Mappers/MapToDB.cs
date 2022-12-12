using RESTaurantBL.Model;
using RESTaurantDLEF.EFModel;
using RESTaurantDLEF.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantDLEF.Mappers
{
    internal class MapToDB
    {
        internal static RestaurantEF MapRestaurant(Restaurant restaurant)
        {
            try
            {
                RestaurantEF restaurantEF = new RestaurantEF(restaurant.Name, restaurant.Kitchen, restaurant.Email, restaurant.Phone, restaurant.Location.PostalCode, restaurant.Location.City);
                if (!string.IsNullOrWhiteSpace(restaurant.Location.Street)) { restaurantEF.Street = restaurant.Location.Street; }
                if (!string.IsNullOrWhiteSpace(restaurant.Location.Housenumber)) { restaurantEF.HousenumberLabel = restaurant.Location.Housenumber; }
                return restaurantEF;
            } catch (Exception ex)
            {
                throw new MapException("MapRestaurant", ex);
            }
        }
    }
}
