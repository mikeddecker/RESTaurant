using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantDLEF.Exceptions
{
    public class RestaurantRepoException : Exception
    {
        public RestaurantRepoException(string? message) : base(message)
        {
        }

        public RestaurantRepoException(string? message, Exception? innerException) : base(message, innerException)
        {
        }
    }
}
