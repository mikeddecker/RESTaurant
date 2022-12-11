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

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public Location(int postalCode, string city) {
            SetPostalCode(postalCode);
            SetCity(city);
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public int PostalCode { get => postalCode; private set => SetPostalCode(value); }
        public string City { get => city; private set => SetCity(value); }

#pragma warning disable CS8604 // Possible null reference argument.
        public string? Street { get => street; private set => SetStreet(value); }
        public string? Housenumber { get => housenumber; private set => SetHousenumber(value); }
#pragma warning restore CS8604 // Possible null reference argument.


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

