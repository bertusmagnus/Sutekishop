using System.Web.Mvc.Html;
using Suteki.Common.Extensions;

namespace Suteki.Common.UI.FormTypeBinders
{
    public class CheckBoxTypeBinder<T> : IFormTypeBinder<T>
    {
        public bool TryBind(FormTypeBindingContext<T> context)
        {
            if (context.Property.PropertyType == typeof(bool))
            {
                context.RenderTableBeginRow();

                context.Html.WriteLine(context.HtmlHelper.CheckBox(
                    context.Property.HtmlId(), 
                    (bool)context.Property.GetValue(context.Entity, null)));
                
                context.RenderTableEndRow();

                return true;
            }
            return false;
        }
    }
}