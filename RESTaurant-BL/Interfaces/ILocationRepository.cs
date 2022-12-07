using RESTaurant_BL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant_BL.Interfaces {
    public interface ILocationRepository {
        void AddLocation(Location location);
        bool DoesExist(Location location);
    }
}
