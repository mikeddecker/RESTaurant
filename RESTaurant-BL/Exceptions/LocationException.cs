using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantBL.Exceptions {
    public class LocationException : Exception {
        public LocationException(string? message) : base(message) {
        }

        public LocationException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
