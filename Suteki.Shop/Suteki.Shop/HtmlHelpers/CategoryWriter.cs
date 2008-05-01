using System;
using System.Web.UI;
using System.IO;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;
using Suteki.Shop.Controllers;
using Suteki.Shop.Repositories;
using Suteki.Shop.Extensions;

namespace Suteki.Shop.HtmlHelpers
{
    public enum CategoryDisplay
    {
        Edit,
        View
    }

    public class CategoryWriter
    {
        Category rootCategory;
        HtmlHelper htmlHelper;
        CategoryDisplay display;

        public CategoryWriter(Category rootCategory, HtmlHelper htmlHelper, CategoryDisplay display)
        {
            this.rootCategory = rootCategory;
            this.htmlHelper = htmlHelper;
            this.display = display;
        }

        public string Write()
        {
            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter());

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
            foreach (Category category in categories)
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
                return "{0} {1} {2} {3}".With(
                    WriteCategoryLink(category),
                    htmlHelper.ActionLink<CategoryController>(c => c.Edit(category.CategoryId), "Edit"),
                    htmlHelper.UpArrowLink<CategoryController>(c => c.MoveUp(category.CategoryId)),
                    htmlHelper.DownArrowLink<CategoryController>(c => c.MoveDown(category.CategoryId))
                    );
            }
            else
            {
                return WriteCategoryLink(category);
            }
        }

        private string WriteCategoryLink(Category category)
        {
            return htmlHelper.ActionLink<ProductController>(c => c.Index(category.CategoryId), category.Name);
        }
    }
}
