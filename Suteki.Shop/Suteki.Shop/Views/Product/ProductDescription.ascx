<%@ Control Language="C#" Inherits="Suteki.Shop.ViewUserControl<Product>" %>
<div class="productDescription">
	<div class="mainImage">
	<% if(Model.HasMainImage) { %>
		<%= Html.Image("~/ProductPhotos/" + Model.MainImage.MainFileName, Model.Name, new { id = "mainImage" })%>
	<% } else {%>
		<%= Html.Image("~/content/images/nopic-large.jpg",Model.Name)%>
	<%} %>
	</div>

	<div class="imageList">
	<% foreach(var productImage in Model.ProductImages.InOrder()) { %>
		<a href="#"><%= Html.Image("~/ProductPhotos/" + productImage.Image.ThumbFileName, Model.Name)%></a>
	<% } %>
	</div>

	<%= Model.Description %>
</div>