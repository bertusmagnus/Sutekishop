using System;
using System.Linq;
using Suteki.Shop.Extensions;

namespace Suteki.Shop.Repositories
{
    public static class ContentRepositoryExtensions
    {
        public static TextContent WithUrlName(this IQueryable<Content> contents, string urlName)
        {
            TextContent textContent = contents
                .OfType<TextContent>()
                .SingleOrDefault(tc => tc.UrlName.ToLower() == urlName.ToLower());

            if (textContent == null) throw new ApplicationException("Unknown UrlName '{0}'".With(urlName));
            return textContent;
        }
    }
}
