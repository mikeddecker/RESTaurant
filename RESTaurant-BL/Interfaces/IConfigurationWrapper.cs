using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurantBL.Interfaces {
    public interface IConfigurationWrapper {
        List<string> GetKitchenTypes();
        bool ContainsKitchenType(string kitchenType);
    }
}
