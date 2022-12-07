using RESTaurant_BL.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RESTaurant_BL.Model {
    public class Location {
        private int postalCode;
        private string city;
        private string? street;
        private string? housenumber;
        private int locationId;

        public Location(int postalCode, string city) {
            SetPostalCode(postalCode);
            SetCity(city);
        }

        public Location(int id, int postalCode, string city) {
            SetLocationId(id);
            SetPostalCode(postalCode);
            SetCity(city);
        }

        public int LocationId { get => locationId; private set => SetLocationId(value); }
        public int PostalCode { get => postalCode; private set => SetPostalCode(value); }
        public string City { get => city; private set => SetCity(value); }

        public string? Street { get => street; private set => SetStreet(value); }
        public string? Housenumber { get => housenumber; private set => SetHousenumber(value); }


        public void SetLocationId(int id) {
            if (id <= 0) { throw new LocationException("SetLocationId - Id smaller than 1"); }
            locationId = id;
        }

        public void SetPostalCode(int postalCode) {
            if (postalCode < 1000 || postalCode > 9999) { throw new LocationException("SetPostalCode - Postcode must be between 1000 and 9999"); }
            this.postalCode = postalCode;
        }

        public void SetCity(string city) { //Todo : update met Camille van Belleplein
            if (string.IsNullOrWhiteSpace(city)) { throw new LocationException("SetCity - No city filled in"); }
            city = city.Trim().ToLower();
            city = char.ToUpper(city[0]) + city.Substring(1);
            this.city = city;
        }

        public void SetStreet(string street) {
            if (string.IsNullOrWhiteSpace(street)) { throw new LocationException("SetStreet - No street filled in"); }
            street = street.Trim().ToLower();
            street = char.ToUpper(street[0]) + street.Substring(1);
            this.street = street;
        }

        public void SetHousenumber(string housenumber) {
            if (string.IsNullOrWhiteSpace(housenumber)) { throw new LocationException("SetHousnumber - No housenumber filled in"); }
            this.housenumber = housenumber.Trim().ToLower();
        }

    }
}

