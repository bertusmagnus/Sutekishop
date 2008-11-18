<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.Stock.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>Stock</h1>

<div class="columnContainer">

<% using(Html.BeginForm<StockController>(c => c.Update())) { %>

    <%= Html.WriteStock(ViewData.Model.Category) %>

    <%= Html.SubmitButton() %>

<% } %>

</div>

</asp:Content>
