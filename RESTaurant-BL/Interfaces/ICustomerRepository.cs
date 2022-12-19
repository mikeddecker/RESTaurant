using RESTaurantBL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantBL.Interfaces {
    public interface ICustomerRepository {
        Customer AddCustomer(Customer customer);
        bool DoesCustomerExist(Customer customer);
        bool DoesCustomerExist(int customerId);
        Customer GetCustomer(int customerId);
        List<Customer> GetCustomers();
        Customer UpdateCustomer(Customer customer);
    }
}
