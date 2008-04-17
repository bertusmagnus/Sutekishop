<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.Category.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<% if (ViewContext.HttpContext.User.IsInRole("Administrator"))
   { %>
    <p><%= Html.ActionLink<CategoryController>(c => c.New(), "New Category")%></p>
<% } %>

<%= Html.WriteCategories(ViewData.Category) %>

</asp:Content>
