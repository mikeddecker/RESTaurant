using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantBL.Exceptions {
    internal class RestaurantException : Exception {
        public RestaurantException(string? message) : base(message) {
        }

        public RestaurantException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
