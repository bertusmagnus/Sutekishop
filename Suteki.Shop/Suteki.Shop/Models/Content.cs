using System;
using Suteki.Shop.Validation;
using System.Text.RegularExpressions;

namespace Suteki.Shop
{
    public partial class Content : IOrderable, IActivatable
    {
        partial void  OnNameChanged()
        {
            TextContent textContent = this as TextContent;
            if (textContent != null)
            {
                textContent.UrlName = Regex.Replace(Name, @"[^A-Za-z0-9]", "_");
            }
        }

        partial void OnNameChanging(string value)
        {
            value.Label("Name").IsRequired();
        }
    }
}
