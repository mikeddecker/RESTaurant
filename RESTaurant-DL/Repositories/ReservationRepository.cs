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
                return ctx.Reservation.Any(r => r.Table.Tablenumber == reservation.Table.TableNumber && r.Customer.CustomerId == reservation.Customer.CustomerId && ( reservation.Date == r.Date || reservation.Date == halfHourEarlier || reservation.Date == oneHourEarlier));
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

        private void SaveAndClear() {
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
        }

    }
}
