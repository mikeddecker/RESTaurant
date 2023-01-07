using RESTaurantBL.Exceptions;
using RESTaurantBL.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantDLEF.Repositories {
    public class ConfigRepo : IConfigurationWrapper {
        public List<string> GetKitchenTypes() {
            return new List<string>(ConfigurationManager.AppSettings["kitchenTypes"].Split(';'));
        }
        public bool ContainsKitchenType(string kitchen) {
            try {
                return GetKitchenTypes().Contains(kitchen);
            } catch (Exception ex) {
                throw new ConfigurationErrorsException(nameof(ContainsKitchenType), ex);
            }
        }
    }
}
