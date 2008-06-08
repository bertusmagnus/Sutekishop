using System;

namespace Suteki.Shop
{
    public class PostageResult
    {
        public decimal Price { get; set; }
        public bool Phone { get; set; }

        public static PostageResult WithPhone { get { return new PostageResult { Phone = true }; } }
        
        public static PostageResult WithPrice(decimal price)
        {
            return new PostageResult { Phone = false, Price = price };
        }

        public static PostageResult WithDefault(PostZone postZone)
        {
            if (postZone.AskIfMaxWeight)
            {
                return WithPhone;
            }
            else
            {
                return WithPrice(postZone.FlatRate);
            }
        }
    }
}
