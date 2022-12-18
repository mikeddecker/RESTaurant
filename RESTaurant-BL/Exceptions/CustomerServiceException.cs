using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantBL.Exceptions {
    public class CustomerServiceException : Exception {
        public CustomerServiceException(string? message) : base(message) {
        }

        public CustomerServiceException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
