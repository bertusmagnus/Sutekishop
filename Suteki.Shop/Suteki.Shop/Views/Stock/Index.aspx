<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.Stock.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>Stock</h1>

<div class="columnContainer">

<% using(Html.Form<StockController>(c => c.Update())) { %>

    <% foreach(var stockItem in ViewData.StockItems) { %>

    <label for="stockitem_<%= stockItem.SizeId %>"><%= stockItem.Name %></label>
    <%= Html.CheckBox("stockitem_" + stockItem.SizeId, "", "True", stockItem.IsInStock) %>

    <% } %>

    <%= Html.SubmitButton() %>

<% } %>

</div>

</asp:Content>
