using RESTaurantBL.Exceptions;
using RESTaurantBL.Interfaces;
using RESTaurantBL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantBL.Services {
    public class CustomerService {
        private ICustomerRepository _customerRepo;

        public CustomerService(ICustomerRepository customerRepo) {
            _customerRepo = customerRepo;
        }

        public Customer AddCustomer(Customer customer) {
            try {
                if (customer == null) { throw new CustomerServiceException($"{nameof(AddCustomer)} - Customer is null"); }
                if (_customerRepo.DoesCustomerExist(customer)) { throw new CustomerServiceException($"{nameof(AddCustomer)} - Customer already exists"); }
                return _customerRepo.AddCustomer(customer);
            } catch (CustomerServiceException) {
                throw;
            } catch (Exception ex) {
                throw new CustomerServiceException(nameof(AddCustomer), ex);
            }
        }
    }
}
