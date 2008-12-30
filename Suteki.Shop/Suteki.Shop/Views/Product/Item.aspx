<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Item.aspx.cs" Inherits="Suteki.Shop.Views.Product.Item" %>
<%@ Import Namespace="Suteki.Common.Extensions"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<script language="javascript">

function onThumbnailClick(img)
{
    var mainImage = document.getElementById("mainImage");
    var imgUrl = img.src.replace("thumb", "main");
    mainImage.src = imgUrl;
}

</script>


<h1><%= ViewData.Model.Product.Name %>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%= ViewData.Model.Product.Price.ToString("£0.00") %><%= ViewData.Model.Product.IsActiveAsString %></h1>

<% if(Context.User.IsInRole("Administrator")) { %>
    <p><%= Html.ActionLink<ProductController>(c => c.Edit(ViewData.Model.Product.ProductId), "Edit") %></p>
<% } %>

<div class="productDescription">
<div class="mainImage">
<% if(ViewData.Model.Product.HasMainImage) { %>
    <%= Html.Image("~/ProductPhotos/" + ViewData.Model.Product.MainImage.MainFileName, new { id = "mainImage" })%>
<% } %>
</div>

<div class="imageList">
<% foreach(var productImage in ViewData.Model.Product.ProductImages.InOrder()) { %>

    <%= Html.Image("~/ProductPhotos/" + productImage.Image.ThumbFileName, new { onclick = "onThumbnailClick(this)" })%>

<% } %>
</div>

<p><%= ViewData.Model.Product.Description %></p>

</div>


<div class="productOptions">
<% using (Html.BeginForm("Update", "Basket"))
   { %>

    <%= Html.ErrorBox(ViewData.Model)%>

    <% if(ViewData.Model.Product.HasSize) { %>
        <label for="sizeid">Size</label>
        <%= Html.DropDownList("sizeid", new SelectList(ViewData.Model.Product.Sizes.Active(), "SizeId", "NameAndStock" ))%>
    <% } else { %>
        <%= Html.Hidden("sizeid", ViewData.Model.Product.DefaultSize.SizeId.ToString()) %>
        <label><%= ViewData.Model.Product.DefaultSize.NameAndStock %></label>
    <% } %>

    <label for="quantity">Quantity</label>
    <%= Html.DropDownList("quantity", new SelectList(1.To(10).Select(i => new { Value = i }), "Value", "Value")) %>

    <%= Html.SubmitButton("addToBasket", "Add to basket")%>
    
<% } %>
</div>

<p>If an item is out of stock, please email us at 
<a href="mailto:<%= ((Suteki.Shop.Controllers.ControllerBase)this.ViewContext.Controller).BaseControllerService.EmailAddress %>">
<%= ((Suteki.Shop.Controllers.ControllerBase)this.ViewContext.Controller).BaseControllerService.EmailAddress%>
</a>
 so that we can let you know when it will be available.</p>

</asp:Content>
