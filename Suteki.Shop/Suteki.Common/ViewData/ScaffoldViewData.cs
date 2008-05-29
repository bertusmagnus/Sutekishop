using System;
using System.Collections.Generic;
using Suteki.Common.Extensions;

namespace Suteki.Common.ViewData
{
    public class ScaffoldViewData<T> : ViewDataBase
    {
        public IEnumerable<T> Items { get; set; }
        public T Item { get; set; }

        private readonly Dictionary<Type, object> lookupLists = new Dictionary<Type, object>();

        public ScaffoldViewData<T> With(T item)
        {
            this.Item = item;
            return this;
        }

        public ScaffoldViewData<T> With(IEnumerable<T> items)
        {
            this.Items = items;
            return this;
        }

        public ScaffoldViewData<T> WithLookupList(Type entityType, object items)
        {
            lookupLists.Add(entityType, items);
            return this;
        }

        public IEnumerable<TLookup> GetLookUpList<TLookup>()
        {
            if (!lookupLists.ContainsKey(typeof(TLookup)))
            {
                throw new ApplicationException("List of type {0} does not exist in lookup list".With(
                    typeof(TLookup).Name));
            }
            return (IEnumerable<TLookup>)lookupLists[typeof(TLookup)];
        }
    }

    public static class ScaffoldView
    {
        public static ScaffoldViewData<T> Data<T>()
        {
            return new ScaffoldViewData<T>();
        }
    }
}
