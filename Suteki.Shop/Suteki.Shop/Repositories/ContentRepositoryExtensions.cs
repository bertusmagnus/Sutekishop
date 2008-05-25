using System;
using System.Linq;
using Suteki.Shop.Extensions;

namespace Suteki.Shop.Repositories
{
    public static class ContentRepositoryExtensions
    {
        public static Menu GetMainMenu(this IRepository<Content> contentRepository)
        {
            // the static data setup should create a main menu content item with contentId = 1
            Menu mainMenu = contentRepository.GetById(1) as Menu;
            if (mainMenu == null)
            {
                throw new ApplicationException(
                    "Expected Content with ContentId = 1 to be ContentType menu. Check static data.");
            }
            return mainMenu; 
        }

        public static Content WithUrlName(this IQueryable<Content> contents, string urlName)
        {
            Content content = contents
                .SingleOrDefault(tc => tc.UrlName.ToLower() == urlName.ToLower());

            if (content == null) throw new ApplicationException("Unknown UrlName '{0}'".With(urlName));
            return content;
        }

        public static IQueryable<Content> WithParent(this IQueryable<Content> contents, Content parent)
        {
            return contents.WithParent(parent.ContentId);
        }

        public static IQueryable<Content> WithParent(this IQueryable<Content> contents, int parentContentId)
        {
            return contents.Where(c => c.ParentContentId == parentContentId);
        }

        public static ITextContent DefaultText(this IQueryable<Content> contents)
        {
            ITextContent text = contents.InOrder().OfType<ITextContent>().FirstOrDefault();

            if (text == null)
            {
                text = new TextContent
                {
                    ContentTypeId = ContentType.TextContentId,
                    Name = "Default",
                    Text = "No content has been created yet",
                    IsActive = true
                };
            }

            return text;
        }

        public static IQueryable<Menu> Menus(this IQueryable<Content> contents)
        {
            return contents.OfType<Menu>();
        }
    }
}
