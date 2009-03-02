using System.Web.Mvc.Html;
using Suteki.Common.Extensions;

namespace Suteki.Common.UI.FormTypeBinders
{
    public class DropDownListFormTypeBinder<T> : IFormTypeBinder<T>
    {
        public bool TryBind(FormTypeBindingContext<T> context)
        {
            // if the property type is derived from IEntity we need to show a drop down list
            // of the entities that can be used.
            if (context.Property.IsEntity())
            {
                if (context.SelectListCollection != null && context.SelectListCollection.ContainsKey(context.Property.Name))
                {
                    context.RenderTableBeginRow();
                    var selectList = context.SelectListCollection[context.Property.Name];
                    context.Html.WriteLine(context.HtmlHelper.DropDownList(context.Property.HtmlId(), selectList));
                    context.Html.WriteLine(context.HtmlHelper.ValidationMessage(context.Property.HtmlId()));
                    context.RenderTableEndRow();
                }
                return true;
            }
            return false;
        }
    }
}