<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.Product.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Product</h1>
    
    <% if(ViewData.Product.ProductId > 0) { %>
        <%= Html.ActionLink<ProductController>(c => c.Item(ViewData.Product.ProductId), "Preview") %>
    <% } %>

    <%= Html.ErrorBox(ViewData) %>
    <%= Html.MessageBox(ViewData) %>

    <% using(Html.Form("Product", "Update")) { %>

        <%= Html.Hidden("productId", ViewData.Product.ProductId) %>

        <label for="name">Name</label>
        <%= Html.TextBox("name", ViewData.Product.Name) %>
        
        <label for="categoryid">Category</label>
        <%= Html.Select("categoryid", ViewData.Categories, "Name", "CategoryId", ViewData.Product.CategoryId)%>
        
        <label for="description">Description</label>
        <%= Html.TextArea("description", ViewData.Product.Description) %>
        
        <%= Html.SubmitButton() %>

    <% } %>


</asp:Content>
