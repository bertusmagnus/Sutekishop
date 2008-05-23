using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data.Linq.Mapping;

namespace Suteki.Shop.Extensions
{
    public static class ObjectExtensions
    {
        /// <summary>
        /// Returns all the public properties as a list of Name, Value pairs
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static IEnumerable<NameValue<object>> GetProperties(this object item)
        {
            foreach (PropertyInfo property in item.GetType().GetProperties())
            {
                yield return new NameValue<object>(property.Name, () => property.GetValue(item, null));
            }
        }

        public static NameValue<object> GetPrimaryKey(this object item)
        {
            PropertyInfo property = item.GetType().GetPrimaryKey();
            return new NameValue<object>(property.Name, () => property.GetValue(item, null));
        }
    }
}
