using Microsoft.EntityFrameworkCore;
using RESTaurantBL.Interfaces;
using RESTaurantBL.Model;
using RESTaurantDLEF.EFModel;
using RESTaurantDLEF.EFModel;
using RESTaurantDLEF.Exceptions;
using RESTaurantDLEF.Mappers;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantDLEF.Repositories {
    public class RestaurantRepository : IRestaurantRepository {
        private RestaurantContext ctx;
        public RestaurantRepository(string connectionString) {
            ctx = new RestaurantContext(connectionString);
        }

        private void SaveAndClear() {
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
        }

        public Restaurant AddRestaurant(Restaurant restaurant) {
            try {
                RestaurantEF rEF = MapToDB.MapRestaurant(restaurant);
                ctx.Restaurant.Add(rEF);
                SaveAndClear();
                restaurant.SetRestaurantId(rEF.RestaurantId);
                return restaurant;
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(AddRestaurant), ex);
            }
        }

        public bool DoesRestaurantExist(Restaurant restaurant) {
            try {
                RestaurantEF restaurantEF = MapToDB.MapRestaurant(restaurant);
                return ctx.Restaurant.Where(r => r.IsDeleted == false).Any(r => r.Email == restaurantEF.Email && r.Name == restaurantEF.Name);
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(DoesRestaurantExist), ex);
            } finally {
                SaveAndClear();
            }
        }

        public List<Restaurant> GetRestaurants() {
            try {
                return ctx.Restaurant.Include(r => r.Location).Where(r => r.IsDeleted == false).Select(r => MapToDomain.MapRestaurant(r)).ToList();
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(GetRestaurants), ex);
            } finally {
                SaveAndClear();
            }
        }

        public bool DoesRestaurantExist(int restaurantId) {
            try {
                return ctx.Restaurant.Any(r => r.RestaurantId == restaurantId && r.IsDeleted == false);
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(DoesRestaurantExist), ex);
            } finally { SaveAndClear(); }
        }

        public Restaurant GetRestaurant(int restaurantId) {
            try {
                return MapToDomain.MapRestaurant(ctx.Restaurant.Include(r => r.Location).Single(r => r.RestaurantId == restaurantId && r.IsDeleted == false));
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(GetRestaurant), ex);
            } finally {
                SaveAndClear();
            }
        }

        public Restaurant UpdateRestaurant(Restaurant restaurant) {
            try {
                RestaurantEF restaurantEFDB = ctx.Restaurant.Include(r => r.Location).Single(r => r.RestaurantId == restaurant.RestaurantId && r.IsDeleted == false);
                if (restaurantEFDB.Name != restaurant.Name) { restaurantEFDB.Name = restaurant.Name; }
                if (restaurantEFDB.Email != restaurant.Email) { restaurantEFDB.Email = restaurant.Email; }
                if (restaurantEFDB.Phone != restaurant.Phone) { restaurantEFDB.Phone = restaurant.Phone; }
                if (restaurantEFDB.Kitchen != restaurant.Kitchen) { restaurantEFDB.Kitchen = restaurant.Kitchen; }
                if (restaurantEFDB.Location.PostalCode != restaurant.Location.PostalCode) { restaurantEFDB.Location.PostalCode = restaurant.Location.PostalCode; }
                if (restaurantEFDB.Location.City != restaurant.Location.City) { restaurantEFDB.Location.City = restaurant.Location.City; }
                if (restaurantEFDB.Location.Street != restaurant.Location.Street) { restaurantEFDB.Location.Street = restaurant.Location.Street; }
                if (restaurantEFDB.Location.HousenumberLabel != restaurant.Location.Housenumber) { restaurantEFDB.Location.HousenumberLabel = restaurant.Location.Housenumber; }
                return restaurant; // SaveAndClear() in finally does the update
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(UpdateRestaurant), ex);
            } finally {
                SaveAndClear();
            }
        }

        public void DeleteRestaurant(int restaurantId) {
            try {
                RestaurantEF restaurantEFDB = ctx.Restaurant.Include(r => r.Location).Single(r => r.RestaurantId == restaurantId && r.IsDeleted == false);
                restaurantEFDB.IsDeleted = true;
                restaurantEFDB.Location.IsDeleted = true;
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(DeleteRestaurant), ex);
            } finally {
                SaveAndClear();
            }
        }

        public bool HasRestaurantTableNumber(int restaurantId, int tableNumber) {
            try {
                return ctx.Table.Any(t => t.RestaurantId == restaurantId && t.Tablenumber == tableNumber && t.IsDeleted == false);
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(HasRestaurantTableNumber), ex);
            } finally { SaveAndClear(); }
        }

        public void AddTableToRestaurant(int restaurantId, int tableNumber, int seats) {
            try {
                TableEF tEF = new TableEF(restaurantId, tableNumber, seats);
                ctx.Table.Add(tEF);
                SaveAndClear();
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(AddTableToRestaurant), ex);
            } finally {
                SaveAndClear();
            }
        }

        public Dictionary<int, int> GetTablesOfRestaurant(int restaurantId) {
            try {
                List<TableEF> tablesEF = ctx.Table.Where(t => t.RestaurantId == restaurantId && t.IsDeleted == false).OrderBy(t => t.Tablenumber).ToList();
                Dictionary<int, int> tableSeats = new Dictionary<int, int>();
                foreach (TableEF tt in tablesEF) {
                    tableSeats.Add(tt.Tablenumber, tt.Seats);
                }
                return tableSeats;
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(GetTablesOfRestaurant), ex);
            } finally {
                SaveAndClear();
            }
        }

        public void DeleteTableOfRestaurant(int restaurantId, int tablenumber) {
            try {
                TableEF tEF = ctx.Table.Single(t => t.RestaurantId == restaurantId && t.Tablenumber == tablenumber && t.IsDeleted == false);
                tEF.IsDeleted = true;
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(DeleteTableOfRestaurant), ex);
            } finally {
                SaveAndClear();
            }
        }

        public void UpdateTableOfRestaurant(int restaurantId, int tableNumber, int seats) {
            // If we come in this method, we know that seatsamount has changed
            try {
                TableEF tableEFDB = ctx.Table.Single(r => r.RestaurantId == restaurantId && r.Tablenumber == tableNumber && r.IsDeleted == false);
                tableEFDB.Seats = seats;
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(UpdateTableOfRestaurant), ex);
            } finally {
                SaveAndClear();
            }
        }

        public Table GetTableOfRestaurant(int restaurantId, int tableNumber) {
            try {
                TableEF tableEFDB = ctx.Table.Single(r => r.RestaurantId == restaurantId && r.Tablenumber == tableNumber && r.IsDeleted == false);
                return MapToDomain.MapTable(tableEFDB);
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(GetTableOfRestaurant), ex);
            } finally {
                SaveAndClear();
            }
        }

        public List<Restaurant> GetRestaurants(string kitchen, int? postalCode) {
            try {
                // At least one parameter should be filled in
                List<Restaurant> restaurants;
                if (string.IsNullOrWhiteSpace(kitchen)) {
                    // PostalCode is filled in & kitchen not
                    restaurants = ctx.Restaurant.Include(r => r.Location).Where(r => r.Location.PostalCode == postalCode.Value && r.IsDeleted == false).Select(rEF => MapToDomain.MapRestaurant(rEF)).ToList();
                } else if (!postalCode.HasValue) {
                    // kitchen is filled in & postalCode not
                    restaurants = ctx.Restaurant.Include(r => r.Location).Where(r => r.Kitchen == kitchen && r.IsDeleted == false).Select(rEF => MapToDomain.MapRestaurant(rEF)).ToList();
                } else {
                    // Both are filled in
                    restaurants = ctx.Restaurant.Include(r => r.Location).Where(r => r.Location.PostalCode == postalCode.Value && r.Kitchen == kitchen && r.IsDeleted == false).Select(rEF => MapToDomain.MapRestaurant(rEF)).ToList();
                }
                return restaurants;
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(GetTableOfRestaurant), ex);
            } finally {
                SaveAndClear();
            }
        }
    }
}
