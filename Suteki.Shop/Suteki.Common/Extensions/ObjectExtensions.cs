using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Suteki.Common.Models;

namespace Suteki.Common.Extensions
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

        public static void WriteProperties(this object item)
        {
            item.WriteProperty(-1);
        }

        public static void WriteProperty(this object item, int level)
        {
            level++;
            if(item == null)
            {
                Console.WriteLine();
                return;
            }

            var items = item as IEnumerable;
            if (items != null && item.GetType() != typeof(string))
            {
                Console.WriteLine();
                foreach (var child in items)
                {
                    Console.Write("{0}", new string('\t', level));
                    child.WriteProperty(level);
                }
                return;
            }

            var entity = item as IEntity;
            if (entity != null)
            {
                Console.WriteLine("{0}", entity.GetType().Name);
                foreach (var property in entity.GetProperties())
                {
                    Console.Write("{0}{1} : ", new string('\t', level), property.Name);
                    property.Value.WriteProperty(level);
                }
                return;
            }

            Console.WriteLine("{0}", item);
        }
    }
}