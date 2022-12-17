using Microsoft.EntityFrameworkCore;
using RESTaurant_DL.EFModel;
using RESTaurantBL.Interfaces;
using RESTaurantBL.Model;
using RESTaurantDLEF.EFModel;
using RESTaurantDLEF.Exceptions;
using RESTaurantDLEF.Mappers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantDLEF.Repositories
{
    public class RestaurantRepository : IRestaurantRepository
    {
        private RestaurantContext ctx;
        public RestaurantRepository(string connectionString)
        {
            ctx = new RestaurantContext(connectionString);
        }

        private void SaveAndClear()
        {
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
        }

        public Restaurant AddRestaurant(Restaurant restaurant)
        {
            try
            {
                RestaurantEF rEF = MapToDB.MapRestaurant(restaurant);
                ctx.Restaurant.Add(rEF);
                SaveAndClear();
                restaurant.SetRestaurantId(rEF.RestaurantId);
                return restaurant;
            }
            catch (Exception ex)
            {
                throw new RestaurantRepoException("AddRestaurant", ex);
            }
        }

        public bool DoesExist(Restaurant restaurant)
        {
            try
            {
                RestaurantEF restaurantEF = MapToDB.MapRestaurant(restaurant);
                return ctx.Restaurant.Any(r => r.Email == restaurantEF.Email && r.Name == restaurantEF.Name);
            }
            catch (Exception ex)
            {
                throw new RestaurantRepoException("DoesExist", ex);
            }
            finally
            {
                SaveAndClear();
            }
        }

        public List<Restaurant> GetRestaurants()
        {
            try
            {
                return ctx.Restaurant.Select(r => MapToDomain.MapRestaurant(r)).ToList();
            }
            catch (Exception ex)
            {
                throw new RestaurantRepoException("DoesExist", ex);
            }
            finally
            {
                SaveAndClear();
            }
        }

        public bool DoesRestaurantExist(int restaurantId)
        {
            try
            {
                return ctx.Restaurant.Any(r => r.RestaurantId == restaurantId);
            }
            catch (Exception ex)
            {
                throw new RestaurantRepoException("DoesExist", ex);
            }
            finally { SaveAndClear(); }
        }

        public Restaurant GetRestaurant(int restaurantId)
        {
            try
            {
                return MapToDomain.MapRestaurant(ctx.Restaurant.Single(r => r.RestaurantId == restaurantId));
            }
            catch (Exception ex)
            {
                throw new RestaurantRepoException("GetRestaurant", ex);
            }
            finally
            {
                SaveAndClear();
            }
        }

        public Restaurant UpdateRestaurant(Restaurant restaurant)
        {
            try
            {
                RestaurantEF restaurantEFDB = ctx.Restaurant.Single(r => r.RestaurantId == restaurant.RestaurantId);
                if (restaurantEFDB.Name != restaurant.Name) { restaurantEFDB.Name = restaurant.Name; }
                if (restaurantEFDB.Email != restaurant.Email) { restaurantEFDB.Email = restaurant.Email; }
                if (restaurantEFDB.Phone != restaurant.Phone) { restaurantEFDB.Phone = restaurant.Phone; }
                if (restaurantEFDB.Kitchen != restaurant.Kitchen) { restaurantEFDB.Kitchen = restaurant.Kitchen; }
                if (restaurantEFDB.PostalCode != restaurant.Location.PostalCode) { restaurantEFDB.PostalCode = restaurant.Location.PostalCode; }
                if (restaurantEFDB.City != restaurant.Location.City) { restaurantEFDB.City = restaurant.Location.City; }
                if (restaurantEFDB.Street != restaurant.Location.Street) { restaurantEFDB.Street = restaurant.Location.Street; }
                if (restaurantEFDB.HousenumberLabel != restaurant.Location.Housenumber) { restaurantEFDB.HousenumberLabel = restaurant.Location.Housenumber; }
                return MapToDomain.MapRestaurant(restaurantEFDB);
            }
            catch (Exception ex)
            {
                throw new RestaurantRepoException("GetRestaurant", ex);
            }
            finally
            {
                SaveAndClear();
            }
        }

        public void DeleteRestaurant(int restaurantId)
        {
            try
            {
                RestaurantEF restaurantEFDB = ctx.Restaurant.Single(r => r.RestaurantId == restaurantId);
                restaurantEFDB.IsDeleted = true;
            }
            catch (Exception ex)
            {
                throw new RestaurantRepoException("DeleteRestaurant", ex);
            }
            finally
            {
                SaveAndClear();
            }
        }

        public bool HasRestaurantTableNumber(int restaurantId, int tableNumber)
        {
            try
            {
                return ctx.Table.Any(t => t.RestaurantId == restaurantId && t.Tablenumber == tableNumber);
            }
            catch (Exception ex)
            {
                throw new RestaurantRepoException("DoesExist", ex);
            }
            finally { SaveAndClear(); }
        }

        public void AddTableToRestaurant(int restaurantId, int tableNumber, int seats)
        {
            try
            {
                TableEF tEF = new TableEF(restaurantId, tableNumber, seats);
                ctx.Table.Add(tEF);
                SaveAndClear();
            }
            catch (Exception ex)
            {
                throw new RestaurantRepoException("AddTableToRestaurant", ex);
            }
        }

        public Dictionary<int, int> GetTablesOfRestaurant(int restaurantId)
        {
            List<TableEF> tablesEF = ctx.Table.Where(t => t.RestaurantId == restaurantId).OrderBy(t => t.Tablenumber).ToList();
            Dictionary<int ,int> tableSeats = new Dictionary<int ,int>();
            foreach (TableEF tt in tablesEF)
            {
                tableSeats.Add(tt.Tablenumber, tt.Seats);
            }
            return tableSeats;
        }
    }
}
