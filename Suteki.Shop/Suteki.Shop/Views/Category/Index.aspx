<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.Category.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<% if (ViewContext.HttpContext.User.IsInRole("Administrator"))
   { %>
    <p><%= Html.ActionLink<CategoryController>(c => c.New(1), "New Category")%></p>
<% } %>

<%= Html.WriteCategories(ViewData.Model.Category, CategoryDisplay.Edit)%>

</asp:Content>
