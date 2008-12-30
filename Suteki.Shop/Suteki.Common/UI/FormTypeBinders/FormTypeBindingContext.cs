using System.IO;
using System.Reflection;
using System.Web.Mvc;
using System.Web.UI;
using Suteki.Common.Extensions;

namespace Suteki.Common.UI.FormTypeBinders
{
    public class FormTypeBindingContext<T>
    {
        public FormTypeBindingContext(PropertyInfo property, HtmlTextWriter html, HtmlHelper htmlHelper, T entity, SelectListCollection selectListCollection)
        {
            Property = property;
            Html = html;
            HtmlHelper = htmlHelper;
            Entity = entity;
            SelectListCollection = selectListCollection;
        }

        public PropertyInfo Property { get; private set; }
        public HtmlTextWriter Html { get; private set; }
        public HtmlHelper HtmlHelper { get; private set; }
        public T Entity { get; private set; }
        public SelectListCollection SelectListCollection { get; private set; }

        public void RenderTableBeginRow()
        {
            Html.RenderBeginTag(HtmlTextWriterTag.Tr);
            RenderLabel(Html, Property);
            Html.RenderBeginTag(HtmlTextWriterTag.Td);
        }

        public void RenderTableEndRow()
        {
            Html.RenderEndTag(); // td
            Html.RenderEndTag(); // tr
        }

        public void RenderLabel(HtmlTextWriter html, PropertyInfo property)
        {
            html.RenderBeginTag(HtmlTextWriterTag.Td);
            html.AddAttribute(HtmlTextWriterAttribute.For, property.HtmlId());
            html.RenderBeginTag(HtmlTextWriterTag.Label);
            html.Write(property.Name.Pretty());
            html.RenderEndTag();
            html.RenderEndTag();
        }
    }
}