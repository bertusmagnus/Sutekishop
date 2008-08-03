using System;
using System.Collections.Generic;
using Suteki.Common.Extensions;
using Suteki.Common.Validation;

namespace Suteki.Shop
{
    public partial class Contact
    {
        partial void OnFirstnameChanging(string value)
        {
            value.Label("First Name").IsRequired();
        }

        partial void OnLastnameChanging(string value)
        {
            value.Label("Last Name").IsRequired();
        }

        partial void OnAddress1Changing(string value)
        {
            value.Label("Address Line 1").IsRequired();
        }

        partial void OnTownChanging(string value)
        {
//            value.Label("Town").IsRequired();
        }

        partial void OnCountyChanging(string value)
        {
//            value.Label("County").IsRequired();
        }

        partial void OnPostcodeChanging(string value)
        {
//            value.Label("Postcode").IsRequired();
        }

        partial void OnTelephoneChanging(string value)
        {
//            value.Label("Telephone").IsRequired();
        }

        public string Fullname
        {
            get
            {
                return "{0} {1}".With(Firstname, Lastname);
            }
        }

        /// <summary>
        /// Get an enumerator over the contact lines, but only return the ones that are available.
        /// This is a convenience method that allows us to easily write a nice address by simply
        /// foreach(ing) over GetAddressLines:
        /// 
        /// foreach(var line in myContact.GetAddressLines())
        /// {
        ///     Console.WriteLine(line);
        /// }
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetAddressLines()
        {
            yield return Fullname;
            yield return Address1;

            if (!string.IsNullOrEmpty(Address2))
            {
                yield return Address2;
            }

            if (!string.IsNullOrEmpty(Address3))
            {
                yield return Address3;
            }

            if (!string.IsNullOrEmpty(Town))
            {
                yield return Town;
            }

            if (!string.IsNullOrEmpty(County))
            {
                yield return County;
            }

            if (!string.IsNullOrEmpty(Postcode))
            {
                yield return Postcode;
            }

            yield return Country.Name;
        }
    }
}
