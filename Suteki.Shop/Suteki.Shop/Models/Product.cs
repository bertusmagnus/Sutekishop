using System;
using Suteki.Shop.Validation;

namespace Suteki.Shop
{
    public partial class Product
    {
        partial void OnNameChanging(string value)
        {
            value.Label("Name").IsRequired();
        }

        partial void OnDescriptionChanging(string value)
        {
            value.Label("Description").IsRequired();
        }
    }
}
