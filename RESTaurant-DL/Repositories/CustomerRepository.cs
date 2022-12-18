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
                return ctx.Customer.Where(c => c.IsDeleted == false).Any(c => c.Email == cEF.Email && c.Name == cEF.Name);
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
            }
        }

    }
}
