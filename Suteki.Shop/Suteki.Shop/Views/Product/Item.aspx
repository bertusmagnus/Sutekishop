<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Item.aspx.cs" Inherits="Suteki.Shop.Views.Product.Item" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<script language="javascript">

function onThumbnailClick(img)
{
    var mainImage = document.getElementById("mainImage");
    var imgUrl = img.src.replace("thumb", "main");
    mainImage.src = imgUrl;
}

</script>


<h1><%= ViewData.Product.Name %>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%= ViewData.Product.Price.ToString("£0.00") %><%= ViewData.Product.IsActiveAsString %></h1>

<% if(Context.User.IsInRole("Administrator")) { %>
    <p><%= Html.ActionLink<ProductController>(c => c.Edit(ViewData.Product.ProductId), "Edit") %></p>
<% } %>

<div class="productDescription">
<div class="mainImage">
<% if(ViewData.Product.HasMainImage) { %>
    <%= Html.Image("~/ProductPhotos/" + ViewData.Product.MainImage.MainFileName, new { id = "mainImage" })%>
<% } %>
</div>

<div class="imageList">
<% foreach(var productImage in ViewData.Product.ProductImages.InOrder()) { %>

    <%= Html.Image("~/ProductPhotos/" + productImage.Image.ThumbFileName, new { onclick = "onThumbnailClick(this)" })%>

<% } %>
</div>

<p><%= ViewData.Product.Description %></p>

</div>


<div class="productOptions">
<% using (Html.Form("Basket", "Update"))
   { %>

    <%= Html.ErrorBox(ViewData)%>

    <% if(ViewData.Product.HasSize) { %>
        <label for="sizeid">Size</label>
        <%= Html.Select("sizeid", ViewData.Product.Sizes.Active(), "NameAndStock", "SizeId")%>
    <% } else { %>
        <%= Html.Hidden("sizeid", ViewData.Product.DefaultSize.SizeId) %>
    <% } %>

    <label for="quantity">Quantity</label>
    <%= Html.Select("quantity", 1.To(10).Select(i => new { Value = i }), "Value", "Value") %>

    <%= Html.SubmitButton("addToBasket", "Add to basket")%>
    
<% } %>
</div>

</asp:Content>
