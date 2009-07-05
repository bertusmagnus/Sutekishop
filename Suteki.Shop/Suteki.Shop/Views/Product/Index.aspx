<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master"  Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1><%= Model.Category.Name %></h1>

<% if(User.IsAdministrator()) { %>
    <p><%= Html.ActionLink<CategoryController>(c => c.New(ViewData.Model.Category.CategoryId), "New Category")%></p>
    <p><%= Html.ActionLink<ProductController>(c => c.New(ViewData.Model.Category.CategoryId), "New Product") %></p>
<% } %>
<% if(Model.Category.ImageId != null) { %>
    <div class="categoryImage">
	 <%= Html.Image("~/ProductPhotos/" + Model.Category.Image.CategoryFileName, "Category Image") %>
	 </div>
<% } %>

<% foreach (var category in ViewData.Model.Category.Categories.ActiveFor((User)User)) { %>
	<% Html.RenderPartial("SubCategory", category); %>
<% } %>

<% foreach (var product in ViewData.Model.Products) { %>
	<% Html.RenderPartial("Product", ShopView.Data.WithProduct(product).WithCategory(Model.Category)); %>
<% } %>

</asp:Content>
