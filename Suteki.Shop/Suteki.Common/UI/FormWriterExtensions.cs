using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.UI;
using Microsoft.Web.Mvc;
using Suteki.Common.Extensions;
using Suteki.Common.Models;
using Suteki.Common.UI.FormTypeBinders;

namespace Suteki.Common.UI
{
    /// <summary>
    /// Automatically writes out an HTML form based on an entity.
    /// It reflects over an entity type, reads its properties and creates the correct input widgets to populate/update it
    /// Use it like this:
    /// 
    /// &lt;%= Html.ForEntity(ViewData.Model.ArticleSearchCriteria).WithSelectLists(ViewData.Model.SelectLists).WriteForm() %&gt;
    /// 
    /// </summary>
    public static class FormWriterExtensions
    {
        public static IEntityHtmlHelperInfo<T> ForEntity<T>(this HtmlHelper htmlHelper, T entity) where T : class
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (htmlHelper == null) throw new ArgumentNullException("htmlHelper");

            return new EntityHtmlHelperInfo<T>(entity, htmlHelper);
        }
    }

    public interface IEntityHtmlHelperInfo<T> where T : class
    {
        T Entity { get; }
        HtmlHelper HtmlHelper { get; }
        SelectListCollection SelectListCollection { get; }
        IEntityHtmlHelperInfo<T> WithSelectLists(SelectListCollection selectLists);
        string WriteForm();
    }

    public class EntityHtmlHelperInfo<T> : IEntityHtmlHelperInfo<T> where T : class
    {
        public T Entity { get; private set; }
        public HtmlHelper HtmlHelper { get; private set; }
        public SelectListCollection SelectListCollection { get; private set; }

        private List<IFormTypeBinder<T>> formTypeBinders;

        public EntityHtmlHelperInfo(T entity, HtmlHelper htmlHelper)
        {
            if (entity == null) throw new ArgumentNullException("entity");
            if (htmlHelper == null) throw new ArgumentNullException("htmlHelper");

            Entity = entity;
            HtmlHelper = htmlHelper;

            CreateFormTypeBinders();
        }

        private void CreateFormTypeBinders()
        {
            formTypeBinders = new List<IFormTypeBinder<T>>
            {
                new IdFormTypeBinder<T>(),
                new ReadonlyFormTypeBinder<T>(),
                new TextBoxFormTypeBinder<T>(),
                new CheckBoxTypeBinder<T>(),
                new DatePickerFormTypeBinder<T>(),
                new DropDownListFormTypeBinder<T>()
            };
        }

        /// <summary>
        /// Add a collection of SelectLists for the form to render
        /// </summary>
        /// <param name="selectLists">A Collection of SelectLists</param>
        /// <returns></returns>
        public IEntityHtmlHelperInfo<T> WithSelectLists(SelectListCollection selectLists)
        {
            SelectListCollection = selectLists;
            return this;
        }

        /// <summary>
        /// Write the contents of a form based on the Entity type
        /// </summary>
        /// <returns>The completed form</returns>
        public string WriteForm()
        {
            var writer = new StringWriter();
            var html = new HtmlTextWriter(writer);

            html.RenderBeginTag(HtmlTextWriterTag.Table);

            var properties = Entity.GetType().GetProperties();

            foreach (var property in properties)
            {
                var context = new FormTypeBindingContext<T>(
                    property,
                    html,
                    HtmlHelper,
                    Entity,
                    SelectListCollection);

                // try each IFormTypeBinder in turn to see if it can render a control for
                // the current property
                foreach (var formTypeBinder in formTypeBinders)
                {
                    if(formTypeBinder.TryBind(context)) break;
                }
            }

            RenderSubmitButton(html);
            html.RenderEndTag(); // table

            return writer.ToString();
        }

        private void RenderSubmitButton(HtmlTextWriter html)
        {
            html.RenderBeginTag(HtmlTextWriterTag.Tr);

            html.RenderBeginTag(HtmlTextWriterTag.Td);
            html.Write("&nbsp;");
            html.RenderEndTag();

            html.RenderBeginTag(HtmlTextWriterTag.Td);
            html.WriteLine(HtmlHelper.SubmitButton("submit", "OK"));
            html.RenderEndTag();

            html.RenderEndTag();
        }
    }
}