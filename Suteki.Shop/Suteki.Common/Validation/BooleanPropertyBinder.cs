using System.Reflection;

namespace Suteki.Common.Validation
{
    /// <summary>
    /// Binds standard checkboxes to boolean values.
    /// </summary>
    public class BooleanPropertyBinder : IBindProperties
    {
        public void Bind(PropertyInfo property, BindingContext bindingContext)
        {
            var value = bindingContext.GetValue(property.Name);
            if (value != null) return;

            // boolean values like checkboxes don't appear unless checked, so set false by default
            if (property.PropertyType == typeof(bool) && property.CanWrite)
            {
                property.SetValue(bindingContext.Target, false, null);
            }
        }
    }
}