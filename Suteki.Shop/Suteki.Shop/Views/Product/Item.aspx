<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Item.aspx.cs" Inherits="Suteki.Shop.Views.Product.Item" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">


<h1><%= ViewData.Product.Name %></h1>

<% if(Context.User.IsInRole("Administrator")) { %>
    <p><%= Html.ActionLink<ProductController>(c => c.Edit(ViewData.Product.ProductId), "Edit") %></p>
<% } %>

<p><%= ViewData.Product.Description %></p>

</asp:Content>
