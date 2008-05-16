using System;
using System.Text.RegularExpressions;

namespace Suteki.Shop
{
    public partial class Content
    {
        partial void  OnNameChanged()
        {
            TextContent textContent = this as TextContent;
            if (textContent != null)
            {
                textContent.UrlName = Regex.Replace(Name, @"[^A-Za-z0-9]", "_");
            }
        }
    }
}
