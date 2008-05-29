using System;
using Suteki.Common;
using Suteki.Common.Validation;

namespace Suteki.Shop
{
    public partial class Country : IOrderable, IActivatable
    {
        partial void OnNameChanging(string value)
        {
            value.Label("Name").IsRequired();
        }
    }
}
