using System.Web.Mvc;
using System.Web.Mvc.Html;
using Suteki.Common.Extensions;
using Suteki.Common.ViewData;
using Microsoft.Web.Mvc;

namespace Suteki.Common.HtmlHelpers
{
    public static class FormHelper
    {
        public static void AutoForm<T>(this HtmlHelper html, ScaffoldViewData<T> viewData)
        {
            var controllerName = html.ViewContext.Controller.GetType().Name.Replace("Controller", "");
            var response = html.ViewContext.HttpContext.Response;
            var keys = new LinqForeignKeyFinder<T>();

            using(html.BeginForm(controllerName, "Update"))
            {
                foreach (var property in typeof(T).GetProperties())
                {
                    // skip collection properties
                    if(property.PropertyType.IsEnumerable() && property.PropertyType != typeof(string)) continue;

                    // skip foreign key id fields
                    if(keys.IsForeignKey(property)) continue;

                    var propertyValue = (property.GetValue(viewData.Item, null) ?? "").ToString();

                    if (property.IsPrimaryKey())
                    {
                        response.Write(html.Hidden(property.Name, propertyValue));
                        continue;
                    }

                    // foreign keys should be rendered as select lists
                    if (property.IsForeignKey())
                    {
                        var idProperty = typeof (T).GetProperty(property.ForeignKeyIdField());

                        response.Write("<label for=\"{0}\">{1}</label>".With(property.Name, property.Name.Pretty()));
                        response.Write(html.DropDownList(property.ForeignKeyIdField(), 
                            new SelectList(
                                viewData.GetLookupList(property.PropertyType), 
                                property.PropertyType.GetPrimaryKey().Name, 
                                "description", 
                                idProperty.GetValue(viewData.Item, null))));
                        continue;
                    }

                    response.Write("<label for=\"{0}\">{1}</label>".With(property.Name, property.Name.Pretty()));
                    response.Write(html.TextBox(property.Name, propertyValue));
                }

                response.Write(html.SubmitButton());
            }
        }
    }
}