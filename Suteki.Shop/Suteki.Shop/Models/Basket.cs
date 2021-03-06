﻿using System;
using System.Linq;

namespace Suteki.Shop
{
    public partial class Basket
    {
        private PostageResult postageTotal;

        public bool IsEmpty
        {
            get
            {
                return !BasketItems.Any();
            }
        }

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
            if (postages == null)
            {
                throw new ArgumentNullException("postages");
            }

            PostZone postZone = Country.PostZone;

            int totalWeight = (int)BasketItems
                .Sum(bi => bi.TotalWeight);

            Postage postageToApply = postages
                .Where(p => totalWeight <= p.MaxWeight && p.IsActive)
                .OrderBy(p => p.MaxWeight)
                .FirstOrDefault();

            if (postageToApply == null) return postageTotal = PostageResult.WithDefault(postZone);

            decimal multiplier = postZone.Multiplier;
            decimal total = Math.Round(postageToApply.Price * multiplier, 2, MidpointRounding.AwayFromZero);

            return postageTotal = PostageResult.WithPrice(total);
        }
    }
}
