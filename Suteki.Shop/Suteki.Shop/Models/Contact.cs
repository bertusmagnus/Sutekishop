using System;
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
            value.Label("Town").IsRequired();
        }

        partial void OnCountyChanging(string value)
        {
            value.Label("County").IsRequired();
        }

        partial void OnPostcodeChanging(string value)
        {
            value.Label("Postcode").IsRequired();
        }

        partial void OnTelephoneChanging(string value)
        {
            value.Label("Telephone").IsRequired();
        }

        public string Fullname
        {
            get
            {
                return "{0} {1}".With(Firstname, Lastname);
            }
        }
    }
}
