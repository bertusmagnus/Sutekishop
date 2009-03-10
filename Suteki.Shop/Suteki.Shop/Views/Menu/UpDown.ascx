<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<Suteki.Shop.Content>" %>
<td>
	<%= Html.UpArrowLink<CmsController>(c => c.MoveUp(Model.ContentId)) %>
	&nbsp;
	<%= Html.DownArrowLink<CmsController>(c => c.MoveDown(Model.ContentId))%>
</td>

