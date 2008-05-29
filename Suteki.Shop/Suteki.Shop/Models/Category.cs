using Suteki.Common;
using Suteki.Common.Validation;

namespace Suteki.Shop
{
    public partial class Category : IOrderable, IActivatable
    {
        partial void OnNameChanging(string value)
        {
            value.Label("Name").IsRequired();
        }

        public bool HasProducts
        {
            get
            {
                return Products.Count > 0;
            }
        }
    }
}
