using System;
using System.Collections.Generic;
using Suteki.Shop.Extensions;
using Suteki.Shop.Validation;

namespace Suteki.Shop
{
    public partial class Card
    {
        public static IEnumerable<int> Months
        {
            get
            {
                return 1.To(12);
            }
        }

        public static IEnumerable<int> ExpiryYears
        {
            get
            {
                return DateTime.Now.Year.To(DateTime.Now.Year + 8);
            }
        }

        public static IEnumerable<int> StartYears
        {
            get
            {
                return (DateTime.Now.Year - 4).To(DateTime.Now.Year);
            }
        }

        // validation

        partial void OnHolderChanging(string value)
        {
            value.Label("Card Holder").IsRequired();
        }

        partial void OnNumberChanging(string value)
        {
            value.Label("Card Number").IsRequired();
        }

        partial void OnSecurityCodeChanging(string value)
        {
            value.Label("Security Code").IsRequired();
        }
    }
}
