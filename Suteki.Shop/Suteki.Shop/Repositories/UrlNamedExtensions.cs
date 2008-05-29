using System;
using System.Linq;
using Suteki.Common.Extensions;

namespace Suteki.Shop.Repositories
{
    public static class UrlNamedExtensions
    {
        public static T WithUrlName<T>(this IQueryable<T> items, string urlName) where T : IUrlNamed
        {
            T item = items
                .SingleOrDefault(i => i.UrlName.ToLower() == urlName.ToLower());

            if (item == null) throw new ApplicationException("Unknown UrlName '{0}'".With(urlName));
            return item;
        }
    }
}
