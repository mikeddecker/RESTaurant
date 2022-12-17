using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant_BL.Exceptions {
    internal class TableException : Exception {
        public TableException(string? message) : base(message) {
        }

        public TableException(string? message, Exception? innerException) : base(message, innerException) {
        }
    }
}
