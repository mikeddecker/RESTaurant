using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantDLEF.Exceptions {
    public class CustomerRepoException : Exception {
        public CustomerRepoException(string? message) : base(message) {
        }

        public CustomerRepoException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
