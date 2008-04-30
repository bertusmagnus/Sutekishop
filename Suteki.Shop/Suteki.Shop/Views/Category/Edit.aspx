<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.Category.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Category</h1>
    
    <%= Html.ErrorBox(ViewData) %>
    <%= Html.MessageBox(ViewData) %>

    <% using(Html.Form("Category", "Update")) { %>

        <%= Html.Hidden("categoryId", ViewData.Category.CategoryId) %>
        <%= Html.Hidden("position", ViewData.Category.Position) %>

        <label for="name">Name</label>
        <%= Html.TextBox("name", ViewData.Category.Name) %>
        
        <label for="parentid">Parent Category</label>
        <%= Html.Select("parentid", ViewData.Categories, "Name", "CategoryId", ViewData.Category.ParentId)%>
        
        <%= Html.SubmitButton() %>

    <% } %>

</asp:Content>
