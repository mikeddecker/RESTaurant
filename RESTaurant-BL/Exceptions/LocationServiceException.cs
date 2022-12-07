using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant_BL.Exceptions {
    public class LocationServiceException : Exception {
        public LocationServiceException(string? message) : base(message) {
        }

        public LocationServiceException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
