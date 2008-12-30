using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using Suteki.Common.Extensions;
using Suteki.Common.Models;

namespace Suteki.Common.HtmlHelpers
{
    public static class TreeRenderHtmlHelper
    {
        public static string RenderTree<T>(
            this HtmlHelper htmlHelper,
            IEnumerable<T> rootLocations,
            Func<T, string> locationRenderer)
            where T : IComposite<T>
        {
            return new TreeRenderer<T>(rootLocations, locationRenderer).Render();
        }
    }

    public class TreeRenderer<T> where T : IComposite<T>
    {
        private const string tokenisedJQueryReadyFunction =
@"<script type=""text/javascript"">
    $(function() {
        $(""#%htmlId%"").treeview();
    });
</script>";

        private const string htmlIdToken = "%htmlId%";

        private readonly Func<T, string> locationRenderer;
        private readonly IEnumerable<T> rootLocations;
        private HtmlTextWriter writer;
        private bool isFirstUL = true;

        public string HtmlId { get; private set; }

        public TreeRenderer(
            IEnumerable<T> rootLocations,
            Func<T, string> locationRenderer)
        {
            this.rootLocations = rootLocations;
            this.locationRenderer = locationRenderer;
            this.HtmlId = Guid.NewGuid().ToString();
        }

        public string Render()
        {
            writer = new HtmlTextWriter(new StringWriter());

            RenderLocations(rootLocations);
            RenderJQueryReadyFunction();

            return writer.InnerWriter.ToString();
        }

        /// <summary>
        /// Recursively walks the location tree outputing it as hierarchial UL/LI elements
        /// </summary>
        /// <param name="locations"></param>
        private void RenderLocations(IEnumerable<T> locations)
        {
            if (locations == null) return;
            if (locations.Count() == 0) return;

            InUl(() => locations.ForEach(location => InLi(() =>
            {
                writer.Write(locationRenderer(location));
                RenderLocations(location.Children);
            })));
        }

        private void InUl(Action action)
        {
            writer.WriteLine();
            if(isFirstUL)
            {
                writer.AddAttribute("id", HtmlId);
                isFirstUL = false;
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Ul);
            action();
            writer.RenderEndTag();
            writer.WriteLine();
        }

        private void InLi(Action action)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Li);
            action();
            writer.RenderEndTag();
            writer.WriteLine();
        }

        /// <summary>
        /// Use jquery.treeview to render a collapsable tree. Make sure that jquery.treeview.js is
        /// referenced.
        /// http://docs.jquery.com/Plugins/Treeview
        /// </summary>
        private void RenderJQueryReadyFunction()
        {
            var jQueryReadyFunction = tokenisedJQueryReadyFunction.Replace(htmlIdToken, HtmlId);
            writer.WriteLine(jQueryReadyFunction);
        }
    }
}