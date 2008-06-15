<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.Product.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1><%= ViewData.Model.Category.Name %></h1>

<% if(Context.User.IsInRole("Administrator")) { %>
    <p><%= Html.ActionLink<CategoryController>(c => c.New(ViewData.Model.Category.CategoryId), "New Category")%></p>
    <p><%= Html.ActionLink<ProductController>(c => c.New(ViewData.Model.Category.CategoryId), "New Product") %></p>
<% } %>

<% foreach (var category in ViewData.Model.Category.Categories.ActiveFor((User)this.User))
   { %>

    <h2><%= Html.ActionLink<ProductController>(c => c.Index(category.CategoryId), category.Name) %></h2>

    <% if(category.HasProducts) { 
           if (category.Products[0].HasMainImage)
           { %>
            <%= Html.Image("~/ProductPhotos/" + category.Products[0].MainImage.ThumbFileName)%>
        <% }
       } 
   } %>

<% foreach (var product in ViewData.Model.Products)
   { %>

    <div onclick="location.href='<%= Url.Action("Item", "Product", new { UrlName = product.UrlName }) %>'" class="product">

        <div><%= product.Name %></div>

        <% if(product.HasMainImage) { %>
            <%= Html.Image("~/ProductPhotos/" + product.MainImage.ThumbFileName) %>
        <% } %>
        
        <% if(Context.User.IsInRole("Administrator")) { %>
            <br />
            <%= Html.UpArrowLink<ProductController>(c => c.MoveUp(ViewData.Model.Category.CategoryId, product.Position)) %>
            <%= Html.DownArrowLink<ProductController>(c => c.MoveDown(ViewData.Model.Category.CategoryId, product.Position)) %>
        <% } %>
    </div>
    </a>

<% } %>

</asp:Content>
