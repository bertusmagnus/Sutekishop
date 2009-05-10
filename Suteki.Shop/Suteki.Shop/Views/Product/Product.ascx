<%@ Control Language="C#" Inherits="Suteki.Shop.ViewUserControl<ShopViewData>" %>

<% if(Model.Product.HasMainImage) { %>
    <a href="<%= Url.Action<ProductController>(c=>c.Item(Model.Product.UrlName)) %>">
	<%= Html.Image("~/ProductPhotos/" + Model.Product.MainImage.ThumbFileName, Model.Product.Name) %></a>
<% } else {%>
	<a href="<%= Url.Action<ProductController>(c=>c.Item(Model.Product.UrlName)) %>">
	<%= Html.Image("~/Content/Images/scaffold/nopic.jpg",Model.Product.Name) %></a>
<% } %>
<% if(Context.User.IsAdministrator()) { %>
<div class="reorder-links">
    <%= Html.UpArrowLink<ProductController>(c => c.MoveUp(Model.Category.CategoryId, Model.Product.Position)) %>
    <%= Html.DownArrowLink<ProductController>(c => c.MoveDown(Model.Category.CategoryId, Model.Product.Position)) %>
    <%= Html.Tick(Model.Product.IsActive) %>
</div>
<% } %>
<span class="productName">
	<%= Html.ActionLink<ProductController>(c => c.Item(Model.Product.UrlName), Model.Product.Name) %>
</span>