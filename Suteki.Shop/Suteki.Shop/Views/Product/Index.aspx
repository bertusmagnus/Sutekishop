<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master"  Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1><%= Model.Category.Name %></h1>

<% if(User.IsAdministrator()) { %>
    <p><%= Html.ActionLink<CategoryController>(c => c.New(ViewData.Model.Category.CategoryId), "New Category")%></p>
    <p><%= Html.ActionLink<ProductController>(c => c.New(ViewData.Model.Category.CategoryId), "New Product") %></p>
<% } %>

<% foreach (var category in ViewData.Model.Category.Categories.ActiveFor((User)User)) { %>
	<% Html.RenderPartial("SubCategory", category); %>
<% } %>

<% foreach (var product in ViewData.Model.Products) { %>
	<% Html.RenderPartial("Product", product); %>
<% } %>

</asp:Content>
