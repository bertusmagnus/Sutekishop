<%@ Control Language="C#" Inherits="Suteki.Shop.ViewUserControl<Category>" %>
<div class ="categoryBox">
	<h2><%= Html.ActionLink<ProductController>(c => c.Index(Model.CategoryId), Model.Name) %></h2>
	<% if(Model.HasProducts && Model.HasMainImage) { %>
		<div onclick="location.href='<%= Url.Action<ProductController>(c =>c.Index(Model.CategoryId)) %>'">
			<%= Html.Image("~/ProductPhotos/" + Model.MainImage.ThumbFileName)%>
		</div>
	<% } %>
</div>