using System;
using System.Reflection;
using System.Data.Linq.Mapping;

namespace Suteki.Common.Extensions
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
    }
}
