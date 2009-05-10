<%@ Control Language="C#" Inherits="Suteki.Shop.ViewUserControl<Category>" %>
<%--
-<div onclick="location.href='<%= Url.Action<ProductController>(c=>c.Index(Model.CategoryId)) %>'" class="product">
    <div><%= Model.Name %></div>

    <% if(Model.HasMainImage) { %>
        <%= Html.Image("~/ProductPhotos/" + Model.MainImage.ThumbFileName, Model.Name) %>
    <% } %>
    
    <% if (Context.User.IsAdministrator()) { %>
		<br />
		Active: <%= Html.Tick(Model.IsActive) %>
    <% } %>
</div>
--%>

<% if(Model.HasMainImage) { %>
    <a href="<%= Url.Action<ProductController>(c=>c.Index(Model.CategoryId)) %>">
	<%= Html.Image("~/ProductPhotos/" + Model.MainImage.ThumbFileName, Model.Name) %></a>
<% } else {%>
	<a href="<%= Url.Action<ProductController>(c=>c.Index(Model.CategoryId)) %>">
	<%= Html.Image("~/Content/Images/scaffold/nopic.jpg", Model.Name) %></a>
<% } %>
<% if(Context.User.IsAdministrator()) { %>
<div class="reorder-links">
    Active: <%= Html.Tick(Model.IsActive) %>
</div>
<% } %>
<span class="productName">
	<%= Html.ActionLink<ProductController>(c => c.Index(Model.CategoryId), Model.Name) %>
</span>