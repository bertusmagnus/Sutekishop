using System;
using System.Linq;
using Suteki.Common.Validation;

namespace Suteki.Shop
{
    public partial class Order
    {
        partial void OnEmailChanging(string value)
        {
            value.Label("Email").IsRequired().IsEmail();
        }

        public Contact PostalContact
        {
            get
            {
                if (UseCardHolderContact) return Contact;
                return Contact1;
            }
        }

        public string DispatchedDateAsString
        {
            get
            {
                if (IsDispatched) return DispatchedDate.ToShortDateString();
                return "&nbsp;";
            }
        }

        public string UserAsString
        {
            get
            {
                if (UserId.HasValue)
                {
                    return User.Email;
                }
                return "&nbsp;";
            }
        }

        public bool IsCreated { get { return OrderStatusId == OrderStatus.CreatedId; } }
        public bool IsDispatched { get { return OrderStatusId == OrderStatus.DispatchedId; } }
        public bool IsRejected { get { return OrderStatusId == OrderStatus.RejectedId; } }
    }
}
