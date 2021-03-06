﻿using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace Suteki.Shop
{
    public partial class ActionContent
    {
        public override MvcHtmlString Link(System.Web.Mvc.HtmlHelper htmlHelper)
        {
            return htmlHelper.ActionLink(Name, Action, Controller);
        }

		public override string Url(System.Web.Mvc.UrlHelper urlHelper) 
		{
			return urlHelper.Action(Action, Controller);
		}
    }
}
