using System;
using System.ComponentModel;
using System.Reflection;

namespace Suteki.Common.Validation
{
    /// <summary>
    /// Binds simple properties (that a type converter can be found for)
    /// </summary>
    public class SimplePropertyBinder : IBindProperties
    {
        public void Bind(PropertyInfo property, BindingContext bindingContext)
        {
            var stringValue = bindingContext.GetValue(property.Name);
            if (stringValue == null) return;

            TypeConverter converter = TypeDescriptor.GetConverter(property.PropertyType);

            if (!converter.CanConvertFrom(typeof(string)))
            {
                throw new FormatException("No type converter available for type: " + property.PropertyType);
            }

            // clean boolean input
            if (property.PropertyType == typeof(bool))
            {
                stringValue = (stringValue.Contains("true") || stringValue.Contains("True")) ? "true" : "false";
            }

            var value = converter.ConvertFrom(stringValue);
            property.SetValue(bindingContext.Target, value, null);
        }
    }
}