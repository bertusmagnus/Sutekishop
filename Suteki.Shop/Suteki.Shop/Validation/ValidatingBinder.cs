using System;
using System.Collections.Specialized;
using System.Reflection;
using System.Web.Mvc;
using System.ComponentModel;
using System.Text;

namespace Suteki.Shop.Validation
{
    public class ValidatingBinder
    {
        public static void UpdateFrom(object target, NameValueCollection values)
        {
            UpdateFrom(target, values, null);
        }

        public static void UpdateFrom(object target, NameValueCollection values, string objectPrefix)
        {
            Type targetType = target.GetType();
            string typeName = targetType.Name;
            PropertyInfo[] properties = targetType.GetProperties();

            StringBuilder exceptionMessage = new StringBuilder();

            foreach (PropertyInfo property in properties)
            {
                string propertyName = property.Name;
                if (!string.IsNullOrEmpty(objectPrefix))
                {
                    propertyName = objectPrefix + "." + property.Name;
                }
                if (values[propertyName] == null)
                {
                    propertyName = typeName + "." + property.Name;
                }
                if (values[propertyName] == null)
                {
                    propertyName = typeName + "_" + property.Name;
                }
                if (values[propertyName] != null)
                {
                    TypeConverter converter = TypeDescriptor.GetConverter(property.PropertyType);
                    string stringValue = values[propertyName];
                    if (!converter.CanConvertFrom(typeof(string)))
                    {
                        throw new FormatException("No type converter available for type: " + property.PropertyType);
                    }
                    try
                    {
                        object value = converter.ConvertFrom(stringValue);
                        property.SetValue(target, value, null);
                    }
                    catch (Exception exception)
                    {
                        if (exception.InnerException is FormatException || 
                            exception.InnerException is IndexOutOfRangeException)
                        {
                            exceptionMessage.AppendFormat("'{0}' is not a valid value for {1}<br />", stringValue, property.Name);
                        }
                        else if (exception.InnerException is ValidationException)
                        {
                            exceptionMessage.AppendFormat("{0}<br />", exception.InnerException.Message);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            if (exceptionMessage.Length > 0)
            {
                throw new ValidationException(exceptionMessage.ToString());
            }
        }
    }
}
