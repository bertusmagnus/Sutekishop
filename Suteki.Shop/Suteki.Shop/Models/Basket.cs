using System;
using System.Linq;

namespace Suteki.Shop
{
    public partial class Basket
    {
        private PostageResult postageTotal;

        public decimal Total
        {
            get
            {
                return BasketItems.Sum(item => item.Total);
            }
        }

        public string PostageTotal
        {
            get
            {
                if (postageTotal == null) return " - ";
                if (postageTotal.Phone) return "Phone";
                return postageTotal.Price.ToString("£0.00");
            }
        }

        public string TotalWithPostage
        {
            get
            {
                if (postageTotal == null) return " - ";
                if (postageTotal.Phone) return "Phone";
                return (Total + postageTotal.Price).ToString("£0.00");
            }
        }

        public PostageResult CalculatePostage(System.Linq.IQueryable<Postage> postages)
        {
            Contact defaultContact = new Contact
            {
                Country = new Country
                {
                    Name = "Default",
                    PostZone = new PostZone
                    {
                        AskIfMaxWeight = false,
                        Multiplier = 1M,
                        IsActive = true,
                    }
                }
            };

            return CalculatePostage(postages, defaultContact);
        }

        public PostageResult CalculatePostage(System.Linq.IQueryable<Postage> postages, Contact contact)
        {
            if (!postages.Any()) return PostageResult.WithPhone;

            int totalWeight = BasketItems
                .Sum(bi => bi.Size.Product.Weight);

            Postage postageToApply = postages
                .Where(p => totalWeight <= p.MaxWeight && p.IsActive)
                .OrderBy(p => p.MaxWeight)
                .FirstOrDefault();

            if (postageToApply == null)
            {
                if (contact.Country.PostZone.AskIfMaxWeight)
                {
                    return postageTotal = PostageResult.WithPhone;
                }
                else
                {
                    return postageTotal = PostageResult.WithPrice(contact.Country.PostZone.FlatRate);
                }
            }

            decimal multiplier = contact.Country.PostZone.Multiplier;
            decimal total = Math.Round(postageToApply.Price * multiplier, 2, MidpointRounding.AwayFromZero);

            return postageTotal = PostageResult.WithPrice(total);
        }
    }
}
