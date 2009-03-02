using System;
using System.Web.Mvc.Html;
using System.Web.UI;
using Suteki.Common.Extensions;

namespace Suteki.Common.UI.FormTypeBinders
{
    public class DatePickerFormTypeBinder<T> : IFormTypeBinder<T>
    {
        public bool TryBind(FormTypeBindingContext<T> context)
        {
            if (context.Property.PropertyType == typeof(DateTime))
            {
                context.RenderTableBeginRow();

                context.Html.WriteLine(context.HtmlHelper.TextBox(context.Property.HtmlName(), ((DateTime)context.Property.GetValue(context.Entity, null)).ToShortDateString()));
                context.Html.WriteLine(context.HtmlHelper.ValidationMessage(context.Property.HtmlName()));

                RenderDatePicker(context.Html, context.Property.HtmlId());

                context.RenderTableEndRow();

                return true;
            }
            return false;            
        }

        private static void RenderDatePicker(HtmlTextWriter html, string htmlId)
        {
            html.RenderBeginTag(HtmlTextWriterTag.Script);
            html.WriteLine();
            html.WriteLine("$('#" + htmlId + "').datepicker({ dateFormat: 'dd/mm/yy' });");
            html.WriteLine();
            html.RenderEndTag();
        }
    }
}