<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.Order.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<table>
    <tr>
        <th>Product</th>
        <th>Size</th>
        <th>Quantity</th>
        <th>Unit Price</th>
        <th>Total Price</th>
    </tr>
    
    <% foreach(var orderItem in ViewData.Order.OrderItems) { %>
    
    <tr>
        <td><%= orderItem.Size.Product.Name %></td>
        <td><%= orderItem.Size.Name %></td>
        <td><%= orderItem.Quantity %></td>
        <td><%= 0.ToString("£0.00") %></td>
        <td><%= 0.ToString("£0.00")%></td>
    </tr>
    
    <% } %>
</table>

</asp:Content>
