<%@ Control Language="C#" Inherits="Suteki.Shop.ViewUserControl<Product>" %>
<div class="productOptions">
<% using (Html.BeginForm<BasketController>(c=>c.Update(null, null))) { %>
    <% if(Model.HasSize) { %>
        <label for="basketItem_SizeId">Size</label>
        <%= Html.DropDownList("basketItem.SizeId", new SelectList(Model.Sizes.Active(), "SizeId", "NameAndStock" ))%>
    <% } else { %>
        <%= Html.Hidden("basketItem.SizeId", Model.DefaultSize.SizeId.ToString()) %>
        <label><%= Model.DefaultSize.NameAndStock %></label>
    <% } %>

    <label for="quantity">Quantity</label>
    <%= Html.DropDownList("basketItem.Quantity", new SelectList(1.To(10).Select(i => new { Value = i }), "Value", "Value")) %>
    <%= Html.SubmitButton("addToBasket", "Add to basket")%>
<% } %>
</div>