using RESTaurant_BL.Exceptions;
using RESTaurant_BL.Interfaces;
using RESTaurant_BL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant_BL.Services {
    public class LocationService {
        private ILocationRepository locationRepo;

        public LocationService(ILocationRepository locationRepo) {
            this.locationRepo = locationRepo;
        }

        public void AddLocation(Location location) {
            try {
                if (location == null) { throw new LocationServiceException("AddLocation - Location is null"); }
                if (locationRepo.DoesExist(location)) { throw new LocationServiceException("AddLocation - Location already exists"); }
                locationRepo.AddLocation(location);
            } catch (LocationServiceException) {
                throw;
            } catch (Exception ex) {
                throw new LocationServiceException("AddLocation", ex);
            }
        }
    }
}
