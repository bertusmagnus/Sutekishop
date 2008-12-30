using System.Reflection;
using Suteki.Common.Models;

namespace Suteki.Common.UI.FormTypeBinders
{
    public class IdFormTypeBinder<T> : IFormTypeBinder<T>
    {
        public bool TryBind(FormTypeBindingContext<T> context)
        {
            return IsEntityProperty(context.Property);
        }

        private static bool IsEntityProperty(PropertyInfo property)
        {
            if (typeof(IEntity).IsAssignableFrom(property.DeclaringType))
            {
                return (property.Name == "Id" || property.Name == "IsNew");
            }
            return false;
        }
    }
}