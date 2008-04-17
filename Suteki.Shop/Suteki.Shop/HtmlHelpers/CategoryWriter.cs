using System;
using System.Web.UI;
using System.IO;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Mvc;
using Suteki.Shop.Controllers;

namespace Suteki.Shop.HtmlHelpers
{
    public class CategoryWriter
    {
        Category rootCategory;
        HtmlHelper htmlHelper;
        User user;

        public CategoryWriter(Category rootCategory, HtmlHelper htmlHelper)
        {
            this.rootCategory = rootCategory;
            this.htmlHelper = htmlHelper;
            this.user = htmlHelper.ViewContext.HttpContext.User as User;
        }

        public string Write()
        {
            HtmlTextWriter writer = new HtmlTextWriter(new StringWriter());

            WriteCategories(writer, rootCategory.Categories);

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
                WriteCategories(writer, category.Categories);
                writer.RenderEndTag();
            }

            if (!first) writer.RenderEndTag();
        }

        private string WriteCategory(Category category)
        {
            if (user.IsAdministrator)
            {
                return string.Format("{0} {1}",
                    WriteCategoryLink(category),
                    htmlHelper.ActionLink<CategoryController>(c => c.Edit(category.CategoryId), "Edit"));
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
