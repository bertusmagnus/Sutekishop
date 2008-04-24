<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="Suteki.Shop.Views.Order.Checkout" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Checkout</h1>
    
    <%= Html.ErrorBox(ViewData) %>
    <%= Html.MessageBox(ViewData) %>

    <% using (Html.Form("Order", "Buy"))
       { %>

        <%= Html.Hidden("orderid", ViewData.Order.OrderId)%>

        <label for="firstname">First Name</label>
        <%= Html.TextBox("firstname")%>
        

    <% } %>
</asp:Content>
