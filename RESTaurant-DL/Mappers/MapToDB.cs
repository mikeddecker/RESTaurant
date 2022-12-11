using RESTaurant_BL.Model;
using RESTaurant_DL.EFModel;
using RESTaurant_DL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant_DL.Mappers
{
    internal class MapToDB
    {
        internal static RestaurantEF MapRestaurant(Restaurant restaurant)
        {
            try
            {
                RestaurantEF restaurantEF = new RestaurantEF(restaurant.RestaurantId, restaurant.Name, restaurant.Kitchen, restaurant.Email, restaurant.Phone, restaurant.Location.PostalCode, restaurant.Location.City);
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
