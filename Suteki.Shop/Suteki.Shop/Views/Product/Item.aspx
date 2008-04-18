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


<h1><%= ViewData.Product.Name %></h1>

<% if(Context.User.IsInRole("Administrator")) { %>
    <p><%= Html.ActionLink<ProductController>(c => c.Edit(ViewData.Product.ProductId), "Edit") %></p>
<% } %>

<div class="mainImage">
<% if(ViewData.Product.HasMainImage) { %>
    <%= Html.Image("~/ProductPhotos/" + ViewData.Product.MainImage.MainFileName, new { id = "mainImage" })%>
<% } %>
</div>

<p><%= ViewData.Product.Description %></p>

<div class="imageList">
<% foreach(var productImage in ViewData.Product.ProductImages) { %>

    <%= Html.Image("~/ProductPhotos/" + productImage.Image.ThumbFileName, new { onclick = "onThumbnailClick(this)" })%>

<% } %>
</div>

</asp:Content>
