using System.Web.Mvc.Html;
using Suteki.Common.Extensions;

namespace Suteki.Common.UI.FormTypeBinders
{
    public class TextBoxFormTypeBinder<T> : IFormTypeBinder<T>
    {
        public bool TryBind(FormTypeBindingContext<T> context)
        {
            if (context.Property.PropertyType == typeof(int) ||
                context.Property.PropertyType == typeof(string) ||
                context.Property.PropertyType == typeof(decimal))
            {
                context.RenderTableBeginRow();

                context.Html.WriteLine(context.HtmlHelper.TextBox(context.Property.HtmlId(),
                                                                  (context.Property.GetValue(context.Entity, null) ?? "")
                                                                      .ToString()));
                context.Html.WriteLine(context.HtmlHelper.ValidationMessage(context.Property.HtmlId()));

                context.RenderTableEndRow();

                return true;
            }
            return false;
        }
    }
}