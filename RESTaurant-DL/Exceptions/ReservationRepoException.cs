using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantDLEF.Exceptions {
    public class ReservationRepoException : Exception {
        public ReservationRepoException(string? message) : base(message) {
        }

        public ReservationRepoException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
