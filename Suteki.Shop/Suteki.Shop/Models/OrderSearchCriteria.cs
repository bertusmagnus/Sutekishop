using System;

namespace Suteki.Shop
{
    public class OrderSearchCriteria
    {
        public int OrderId { get; set; }
        public string Email { get; set; }
        public string Postcode { get; set; }
        public string Lastname { get; set; }
        public int OrderStatusId { get; set; }

//        public bool HasOrderId { get { return OrderId != 0; } }
//        public bool HasEmail { get { return !string.IsNullOrEmpty(Email); } }
//        public bool HasPostcode { get { return !string.IsNullOrEmpty(Postcode); } }
//        public bool HasLastname { get { return !string.IsNullOrEmpty(Lastname); } }
//        public bool HasOrderStatusId { get { return OrderStatusId != 0; } }
    }
}
