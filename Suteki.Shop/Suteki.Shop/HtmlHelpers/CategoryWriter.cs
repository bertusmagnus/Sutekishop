using System.Web.UI;
using System.IO;
using System.Collections.Generic;
using System.Web.Mvc;
using Microsoft.Web.Mvc;
using Suteki.Common.Extensions;
using Suteki.Common.HtmlHelpers;
using Suteki.Common.Repositories;
using Suteki.Shop.Controllers;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.HtmlHelpers
{
    public enum CategoryDisplay
    {
        Edit,
        View
    }

    public class CategoryWriter
    {
        readonly Category rootCategory;
        readonly HtmlHelper htmlHelper;
        readonly CategoryDisplay display;

        public CategoryWriter(Category rootCategory, HtmlHelper htmlHelper, CategoryDisplay display)
        {
            this.rootCategory = rootCategory;
            this.htmlHelper = htmlHelper;
            this.display = display;
        }

        public string Write()
        {
            var writer = new HtmlTextWriter(new StringWriter());

            if (display == CategoryDisplay.View)
            {
                writer.AddAttribute("class", "category");
            }

            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            WriteCategories(writer, rootCategory.Categories.InOrder());

            writer.RenderEndTag();

            return writer.InnerWriter.ToString();
        }

        private void WriteCategories(HtmlTextWriter writer, IEnumerable<Category> categories)
        {
            
            
            bool first = true;
            foreach (Category category in categories.ActiveFor(htmlHelper.CurrentUser()))
            {
                if (first)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Ul);
                    first = false;
                }

                writer.RenderBeginTag(HtmlTextWriterTag.Li);
                writer.Write(WriteCategory(category));
                WriteCategories(writer, category.Categories.InOrder());
                writer.RenderEndTag();
            }

            if (!first) writer.RenderEndTag();
        }

        private string WriteCategory(Category category)
        {
            if (display == CategoryDisplay.Edit)
            {
                return "{0} {1} {2} {3} {4}".With(
                    WriteCategoryLink(category),
                    htmlHelper.ActionLink<CategoryController>(c => c.Edit(category.CategoryId), "Edit"),
                    htmlHelper.Tick(category.IsActive),
                    htmlHelper.UpArrowLink<CategoryController>(c => c.MoveUp(category.CategoryId)),
                    htmlHelper.DownArrowLink<CategoryController>(c => c.MoveDown(category.CategoryId))
                    );
            }

            return WriteCategoryLink(category);
        }

        private string WriteCategoryLink(Category category)
        {
            return htmlHelper.ActionLink<ProductController>(c => c.Index(category.CategoryId), category.Name);
        }
    }
}
