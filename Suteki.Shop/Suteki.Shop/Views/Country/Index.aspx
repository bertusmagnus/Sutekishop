<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.master" Inherits="Suteki.Shop.ViewPage<ScaffoldViewData<Suteki.Shop.Country>>" %>
<%@ Import Namespace="MvcContrib.Pagination"%>
<%@ Import Namespace="Suteki.Common.ViewData"%>
<%@ Import Namespace="MvcContrib.UI.Pager" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div class="admin-form">
	<h1>Countries</h1>
	<div class="message"><%= TempData["message"] %></div>
	<%= Html.Grid(Model.Items).Columns(column => {
			column.For(x => Html.ActionLink<CountryController>(c => c.Edit(x.CountryId), x.Name)).DoNotEncode().Named("Name").HeaderAttributes(@class => "wide");
			column.For(x => Html.Tick(x.IsActive)).DoNotEncode().Named("Active").HeaderAttributes(@class => "thin");
			column.For(x => x.PostZone.Name).Named("Post Zone");
			column.For(x => Html.UpArrowLink<CountryController>(c => c.MoveUp(x.Position, 1))).DoNotEncode().Named("&nbsp;");
			column.For(x => Html.DownArrowLink<CountryController>(c => c.MoveDown(x.Position, 1))).DoNotEncode().Named("&nbsp;");
		}) %>
	<%= Html.Pager((IPagination)Model.Items) %>
	<p id="addCountry"><%= Html.ActionLink<CountryController>(c => c.New(), "Add  a new country") %></p>
</div>
</asp:Content>
