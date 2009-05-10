<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master"  Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<% if(User.IsAdministrator()) { %>
	<ul id="admin-submenu">
		<li><%= Html.ActionLink<CategoryController>(c => c.New(Model.Category.CategoryId), "New Category")%></li>
		<li><%= Html.ActionLink<ProductController>(c => c.New(Model.Category.CategoryId), "New Product") %></li>
    </ul>
<% } %>

<h1><%= Model.Category.Name %></h1>

<% var subCategories = Model.Category.Categories.ActiveFor((User)User).ToList(); %>

<% if(Model.HasProducts || subCategories.Count() > 0){ %>
	<ul id="productGrid">
	<% foreach (var category in subCategories) { %>
		<li>
			<% Html.RenderPartial("SubCategory", category); %>
		</li>
	<% } %>
	<% foreach (var product in Model.Products) { %>
		<li>
			<% Html.RenderPartial("Product", ShopView.Data.WithProduct(product).WithCategory(Model.Category)); %>
		</li>
	<% } %>
	</ul>
<%}%>
</asp:Content>
