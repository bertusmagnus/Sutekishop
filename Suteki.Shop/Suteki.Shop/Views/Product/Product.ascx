<%@ Control Language="C#" Inherits="Suteki.Shop.ViewUserControl<Product>" %>
<div onclick="location.href='<%= Url.Action<ProductController>(c=>c.Item(Model.UrlName)) %>'" class="product">
    <div><%= Model.Name %></div>

    <% if(Model.HasMainImage) { %>
        <%= Html.Image("~/ProductPhotos/" + Model.MainImage.ThumbFileName) %>
    <% } %>
    
    <% if(Context.User.IsAdministrator()) { %>
        <br />
        <%= Html.UpArrowLink<ProductController>(c => c.MoveUp(ViewData.Model.Category.CategoryId, Model.Position)) %>
        <%= Html.DownArrowLink<ProductController>(c => c.MoveDown(ViewData.Model.Category.CategoryId, Model.Position)) %>
    <% } %>
</div>
