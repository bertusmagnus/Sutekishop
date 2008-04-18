﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.Product.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Product</h1>
    
    <% if(ViewData.Product.ProductId > 0) { %>
        <%= Html.ActionLink<ProductController>(c => c.Item(ViewData.Product.ProductId), "Preview") %>
    <% } %>

    <%= Html.ErrorBox(ViewData) %>
    <%= Html.MessageBox(ViewData) %>

    <% using (Html.Form("Product", "Update", FormMethod.Post, new Dictionary<string, object> { { "enctype", "multipart/form-data" } } ))
       { %>

        <%= Html.Hidden("productId", ViewData.Product.ProductId)%>

        <label for="name">Name</label>
        <%= Html.TextBox("name", ViewData.Product.Name)%>
        
        <label for="categoryid">Category</label>
        <%= Html.Select("categoryid", ViewData.Categories, "Name", "CategoryId", ViewData.Product.CategoryId)%>
        
        <label for="description">Description</label>
        <%= Html.TextArea("description", ViewData.Product.Description)%>
        
        <div class="imageList">
        <% foreach(var productImage in ViewData.Product.ProductImages) { %>

            <%= Html.Image("~/ProductPhotos/" + productImage.Image.ThumbFileName) %>

        <% } %>
        </div>
        
        <h3>Upload Photos</h3>
        
        <% for (int i = 0; i < 5; i++)
           { %>
            <input type="file" id="image_<%= i.ToString() %>" name="image_<%= i.ToString() %>" />
        <% } %>
        <%= Html.SubmitButton()%>

    <% } %>


</asp:Content>
