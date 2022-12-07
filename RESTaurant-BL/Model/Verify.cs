using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RESTaurant_BL.Model {
    public static class Verify {
        public static bool IsValidEmailSyntax(string email) {
            if (string.IsNullOrWhiteSpace(email)) { return false; }
            Regex emailCheck = new Regex(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$", RegexOptions.CultureInvariant | RegexOptions.Singleline);
            if (emailCheck.IsMatch(email)) return true;
            return false;
        }

        public static bool IsValidPhoneNumberBE(string phoneNumberStringToValidate) {
            var phoneNumberUtil = PhoneNumbers.PhoneNumberUtil.GetInstance();
            var phoneNumberBE = phoneNumberUtil.Parse(phoneNumberStringToValidate, "BE");
            return phoneNumberUtil.IsValidNumber(phoneNumberBE);
        }
    }
}
