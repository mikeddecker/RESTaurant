using Microsoft.EntityFrameworkCore;
using RESTaurantBL.Interfaces;
using RESTaurantBL.Model;
using RESTaurantDLEF.EFModel;
using RESTaurantDLEF.Exceptions;
using RESTaurantDLEF.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantDLEF.Repositories {
    public class ReservationRepository : IReservationRepository {
        private RestaurantContext ctx;

        public ReservationRepository(string connectionstring) {
            ctx = new RestaurantContext(connectionstring);
        }

        public Reservation AddReservation(Reservation reservation) {
            try {
                RestaurantEF restaurantEF = ctx.Restaurant.Single(r => r.RestaurantId == reservation.Restaurant.RestaurantId);
                CustomerEF customerEF = ctx.Customer.Single(c => c.CustomerId == reservation.Customer.CustomerId);
                TableEF tableEF = ctx.Table.Single(t => t.RestaurantId == reservation.Restaurant.RestaurantId && t.Tablenumber == reservation.Table.TableNumber);
                //TableEF tableEF = ctx.Table.Single()
                ReservationEF reservationEF = new ReservationEF(restaurantEF, customerEF, tableEF, reservation.Seats, reservation.Date);
                ctx.Reservation.Add(reservationEF);
                SaveAndClear();
                reservation.SetReservationId(reservationEF.ReservationId);
                return reservation;
            } catch (Exception ex) {
                throw new CustomerRepoException(nameof(AddReservation), ex);
            }
        }


        public bool DoesReservationExist(Reservation reservation) {
            try {
                // Exists on restaurant, customer, date & time?
                return ctx.Reservation.Any(r => r.Restaurant.RestaurantId == reservation.Restaurant.RestaurantId && r.Customer.CustomerId == reservation.Customer.CustomerId && reservation.Date == r.Date);
            } catch (Exception ex) {
                throw new ReservationRepoException(nameof(DoesReservationExist), ex);
            } finally {
                SaveAndClear();
            }
        }

        public bool DoesReservationOverlapCustomer(Reservation reservation) {
            try {
                // Checking if a customer doesn't already have a reservation with another restaurant
                DateTime halfHourEarlier = reservation.Date.Add(new TimeSpan(0, -30, 0));
                DateTime oneHourEarlier = reservation.Date.Add(new TimeSpan(-1, 0, 0));
                return ctx.Reservation.Any(r => r.Table.Tablenumber == reservation.Table.TableNumber && r.Customer.CustomerId == reservation.Customer.CustomerId && (reservation.Date == r.Date || reservation.Date == halfHourEarlier || reservation.Date == oneHourEarlier));
            } catch (Exception ex) {
                throw new ReservationRepoException(nameof(DoesReservationOverlapCustomer), ex);
            } finally {
                SaveAndClear();
            }
        }

        public bool DoesReservationOverlapTable(Reservation reservation) {
            try {
                DateTime halfHourEarlier = reservation.Date.Add(new TimeSpan(0, -30, 0));
                DateTime oneHourEarlier = reservation.Date.Add(new TimeSpan(-1, 0, 0));
                return ctx.Reservation.Any(r => r.Table.Tablenumber == reservation.Table.TableNumber && r.Restaurant.RestaurantId == reservation.Restaurant.RestaurantId && (reservation.Date == r.Date || reservation.Date == halfHourEarlier || reservation.Date == oneHourEarlier));
            } catch (Exception ex) {
                throw new ReservationRepoException(nameof(DoesReservationOverlapTable), ex);
            } finally {
                SaveAndClear();
            }
        }

        public List<Restaurant> GetAvailableRestaurants(DateTime date) {
            try {
                List<Restaurant> restaurantsWithATableAvailable = new List<Restaurant>();

                // Let's already get it's location
                List<RestaurantEF> restaurantEFList = ctx.Restaurant.Include(r => r.Location).Include(r => r.Tables).AsNoTracking().Where(r => r.IsDeleted == false).ToList();
                foreach (RestaurantEF restaurantEF in restaurantEFList) {
                    if (CanIMakeReservationAtRestaurant(restaurantEF, date)) { restaurantsWithATableAvailable.Add(MapToDomain.MapRestaurant(restaurantEF)); }
                }
                return restaurantsWithATableAvailable;
            } catch (Exception ex) {
                throw new ReservationRepoException(nameof(GetReservationsOnDate_Table_Reservation), ex);
            } finally {
                SaveAndClear();
            }
        }

        private bool CanIMakeReservationAtRestaurant(RestaurantEF restaurantEF, DateTime date) {
            try {
                DateTime halfHourEarlier = date.AddMinutes(-30);
                DateTime oneHourEarlier = date.AddHours(-1);
                DateTime halfHourLater = date.AddMinutes(30);
                DateTime oneHourLater = date.AddHours(1);

                int reservationsAtDate = ctx.Reservation.Count(r => r.IsCanceled == false && r.IsDeleted == false && restaurantEF.RestaurantId == r.Restaurant.RestaurantId && (r.Date == date || r.Date == halfHourEarlier || r.Date == halfHourLater || r.Date == oneHourEarlier || r.Date == oneHourLater));
                int nrOfTables = ctx.Table.Count(t => t.IsDeleted == false && t.RestaurantId == restaurantEF.RestaurantId);
                return nrOfTables > reservationsAtDate;


            } catch (Exception ex) {
                throw new ReservationRepoException(nameof(GetReservationsOnDate_Table_Reservation), ex);
            } finally {
                SaveAndClear();
            }
        }

        public Dictionary<int, Reservation> GetReservationsOnDate_Table_Reservation(int restaurantId, DateTime date) {
            try {
                DateTime halfHourEarlier = date.Add(new TimeSpan(0, -30, 0));
                DateTime oneHourEarlier = date.Add(new TimeSpan(-1, 0, 0));
                return ctx.Reservation.Include(r => r.Restaurant).ThenInclude(r => r.Location).Include(r => r.Customer).ThenInclude(c => c.Location).Include(r => r.Table).Where(r => r.IsDeleted == false && r.IsCanceled == false && r.Restaurant.RestaurantId == restaurantId && (r.Date == date || r.Date == halfHourEarlier || r.Date == oneHourEarlier)).Select(r => MapToDomain.MapReservation(r)).ToDictionary(r => r.Table.TableNumber, r => r);
            } catch (Exception ex) {
                throw new ReservationRepoException(nameof(GetReservationsOnDate_Table_Reservation), ex);
            } finally {
                SaveAndClear();
            }
        }

        private void SaveAndClear() {
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
        }

        public List<Reservation> GetReservationsOfCustomer(int customerId, DateTime beginDate, DateTime endDate) {
            try {
                return ctx.Reservation.Include(r => r.Restaurant).ThenInclude(r => r.Location).Include(r => r.Customer).ThenInclude(c => c.Location).Include(r => r.Table).Where(r => r.IsDeleted == false && r.Customer.CustomerId == customerId && r.Date > beginDate && r.Date < endDate).Select(r => MapToDomain.MapReservation(r)).ToList();
            } catch (Exception ex) {
                throw new ReservationRepoException(nameof(GetReservationsOnDate_Table_Reservation), ex);
            } finally {
                SaveAndClear();
            }
        }

        public bool DoesReservationExist(int reservationId) {
            try {
                return ctx.Reservation.AsNoTracking().Any(r => r.ReservationId == reservationId && r.IsDeleted == false && r.IsCanceled == false);
            } catch (Exception ex) {
                throw new ReservationRepoException(nameof(DoesReservationExist), ex);
            } finally {
                SaveAndClear();
            }
        }

        public void CancelReservation(int reservationId) {
            try {
                ReservationEF reservationEF = ctx.Reservation.Single(r => r.ReservationId == reservationId && r.IsDeleted == false && r.IsCanceled == false);
                reservationEF.IsCanceled = true;
                SaveAndClear(); // Setting it here, so we can catch exceptions
            } catch (Exception ex) {
                throw new ReservationRepoException(nameof(CancelReservation), ex);
            }
        }
    }
}
