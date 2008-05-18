using System;
using Suteki.Shop.Validation;

namespace Suteki.Shop
{
    public partial class TextContent
    {
        partial void OnTextChanging(string value)
        {
            value.Label("Text").IsRequired();
        }
    }
}
