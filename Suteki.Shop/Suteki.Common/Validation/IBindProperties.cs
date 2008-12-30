using System.Reflection;

namespace Suteki.Common.Validation
{
    public interface IBindProperties
    {
        void Bind(PropertyInfo property, BindingContext bindingContext);
    }
}