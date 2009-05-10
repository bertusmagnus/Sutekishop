<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.master"  Inherits="Suteki.Shop.ViewPage<ScaffoldViewData<Postage>>" %>
<%@ Import Namespace="MvcContrib.Pagination"%>
<%@ Import Namespace="Suteki.Common.ViewData"%>
<%@ Import Namespace="MvcContrib.UI.Pager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div class="admin-form">
	<h1>Postage Bands</h1>

	<div class="message"><%= TempData["message"] %></div>
	<p><%= Html.ActionLink<PostageController>(c => c.New(), "New Postage Band") %></p>

	<%= Html.Grid(Model.Items)
		.Columns(column => {
			column.For(x => Html.ActionLink<PostageController>(c => c.Edit(x.PostageId), x.Name)).DoNotEncode().HeaderAttributes(@class => "thin");
			column.For(x => x.MaxWeight).Attributes(@class => "number").HeaderAttributes(@class => "thin number");
			column.For(x => x.Price).Format("{0:C}").Attributes(@class => "number").HeaderAttributes(@class => "thin number");
			column.For(x => Html.Tick(x.IsActive)).DoNotEncode().HeaderAttributes(@class => "thin").Named("Active");
			column.For(x => Html.UpArrowLink<PostageController>(c => c.MoveUp(x.Position, 1))).Named("&nbsp;").DoNotEncode().HeaderAttributes(@class => "arrow");
			column.For(x => Html.DownArrowLink<PostageController>(c => c.MoveDown(x.Position, 1))).Named("&nbsp;").DoNotEncode().HeaderAttributes(@class => "arrow");
		 }) %>
	     
	  <%= Html.Pager((IPagination)Model.Items) %>
</div>
</asp:Content>