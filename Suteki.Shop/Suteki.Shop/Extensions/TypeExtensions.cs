using System;
using System.Reflection;
using System.Data.Linq.Mapping;
using System.Collections.Generic;

namespace Suteki.Shop.Extensions
{
    public static class TypeExtensions
    {
        public static PropertyInfo GetPrimaryKey(this Type entityType)
        {
            foreach (PropertyInfo property in entityType.GetProperties())
            {
                ColumnAttribute[] attributes = (ColumnAttribute[])property.GetCustomAttributes(typeof(ColumnAttribute), true);
                if (attributes.Length == 1)
                {
                    ColumnAttribute columnAttribute = attributes[0];
                    if (columnAttribute.IsPrimaryKey)
                    {
                        if (property.PropertyType != typeof(int))
                        {
                            throw new ApplicationException(string.Format("Primary key, '{0}', of type '{1}' is not int",
                                property.Name, entityType));
                        }
                        return property;
                    }
                }
            }
            throw new ApplicationException(string.Format("No primary key defined for type {0}", entityType.Name));
        }

        public static bool IsLinqEntity(this Type type)
        {
            TableAttribute[] attributes = (TableAttribute[])type.GetCustomAttributes(typeof(TableAttribute), true);
            if (attributes.Length == 1)
            {
                return true;
            }
            return false;
        }

        public static IEnumerable<PropertyInfo> PropertiesWithAttributeOf(this Type type, Type attributeType)
        {
            foreach(PropertyInfo property in type.GetProperties())
            {
                if (property.HasAttribute(attributeType))
                {
                    yield return property;
                }
            }
        }

        public static bool HasAttribute(this PropertyInfo property, Type attributeType)
        {
            object[] attributes = property.GetCustomAttributes(attributeType, true);
            return attributes.Length > 0;
        }
    }
}
