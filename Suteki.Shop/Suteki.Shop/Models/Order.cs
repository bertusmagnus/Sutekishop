using System;
using System.Linq;
using Suteki.Shop.Validation;

namespace Suteki.Shop
{
    public partial class Order
    {
        partial void OnEmailChanging(string value)
        {
            value.Label("Email").IsRequired().IsEmail();
        }

        public PostageResult CalculatePostage(System.Linq.IQueryable<Postage> postages)
        {
            if (Basket == null)
            {
                throw new ApplicationException("Cannot calculate postage when Basket is null");
            }

            if (PostalContact == null || PostalContact.Country == null || PostalContact.Country.PostZone == null)
            {
                    return Basket.CalculatePostage(postages);
            }
                
            return Basket.CalculatePostage(postages, PostalContact);
        }

        public Contact PostalContact
        {
            get
            {
                if (UseCardHolderContact) return Contact;
                return Contact1;
            }
        }
    }
}
