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
            if (!postages.Any()) return postageTotal = PostageResult.WithPhone;

            int totalWeight = BasketItems
                .Sum(bi => bi.Size.Product.Weight);

            Postage postageToApply = postages
                .Where(p => totalWeight <= p.MaxWeight && p.IsActive)
                .OrderBy(p => p.MaxWeight)
                .FirstOrDefault();

            if (postageToApply == null)
            {
                // total weight is greater than any of the defined postage bands so phone
                return postageTotal = PostageResult.WithPhone;
            }

            if (contact.Country.PostZone.AskIfMaxWeight)
            {
                Postage maxPostage = postages.Where(p => p.IsActive).OrderByDescending(p => p.MaxWeight).First();
                if (postageToApply == maxPostage)
                {
                    return postageTotal = PostageResult.WithPhone;
                }
            }

            decimal multiplier = contact.Country.PostZone.Multiplier;
            decimal total = Math.Round(postageToApply.Price * multiplier, 2, MidpointRounding.AwayFromZero);

            return postageTotal = PostageResult.WithPrice(total);
        }
    }
}
