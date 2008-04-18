<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.Product.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1><%= ViewData.Category.Name %></h1>

<% if(Context.User.IsInRole("Administrator")) { %>
    <p><%= Html.ActionLink<ProductController>(c => c.New(ViewData.Category.CategoryId), "New Product") %></p>
<% } %>

<% foreach (var product in ViewData.Products)
   { %>

    <a href="<%= Url.Action("Item", "Product", new { Id = product.ProductId.ToString() }) %>">
    <div class="product">
        <% if(product.HasMainImage) { %>
            <%= Html.Image("~/ProductPhotos/" + product.MainImage.ThumbFileName) %>
        <% } %>
        <%= product.Name %>
    </div>
    </a>

<% } %>

</asp:Content>
