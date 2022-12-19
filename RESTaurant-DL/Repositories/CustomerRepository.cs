using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using RESTaurantBL.Interfaces;
using RESTaurantBL.Model;
using RESTaurantDLEF;
using RESTaurantDLEF.EFModel;
using RESTaurantDLEF.Exceptions;
using RESTaurantDLEF.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantDLEF.Repositories {
    public class CustomerRepository : ICustomerRepository {
        private string _connectionstring;
        private RestaurantContext ctx;

        public CustomerRepository(string connectionstring) {
            ctx = new RestaurantContext(connectionstring);
        }

        private void SaveAndClear() {
            ctx.SaveChanges();
            ctx.ChangeTracker.Clear();
        }
        public bool DoesCustomerExist(Customer customer) {
            try {
                CustomerEF cEF = MapToDB.MapCustomer(customer);
                return ctx.Customer.Any(c => c.IsDeleted == false && c.Email == cEF.Email && c.Name == cEF.Name);
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(DoesCustomerExist), ex);
            } finally {
                SaveAndClear();
            }
        }

        public Customer AddCustomer(Customer c) {
            try {
                CustomerEF cEF = MapToDB.MapCustomer(c);
                ctx.Customer.Add(cEF);
                SaveAndClear();
                c.SetCustomerId(cEF.CustomerId);
                return c;
            } catch (Exception ex) {
                throw new CustomerRepoException(nameof(AddCustomer), ex);
            } finally {
                SaveAndClear();
            }
        }

        public bool DoesCustomerExist(int customerId) {
            try {
                return ctx.Customer.Any(c => c.IsDeleted == false && c.CustomerId == customerId);
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(DoesCustomerExist), ex);
            } finally {
                SaveAndClear();
            }
        }

        public Customer GetCustomer(int customerId) {
            try {
                CustomerEF cEF = ctx.Customer.Include(c => c.Location).Single(c => c.IsDeleted == false && c.CustomerId == customerId);
                return MapToDomain.MapCustomer(cEF);
            } catch (Exception ex) {
                throw new RestaurantRepoException(nameof(GetCustomer), ex);
            } finally {
                SaveAndClear();
            }
        }
    }
}
