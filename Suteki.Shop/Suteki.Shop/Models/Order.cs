using System;
using Suteki.Shop.Validation;

namespace Suteki.Shop
{
    public partial class Order
    {
        partial void OnEmailChanging(string value)
        {
            value.Label("Email").IsRequired().IsEmail();
        }
    }
}
