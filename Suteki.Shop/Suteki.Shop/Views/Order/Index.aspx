<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.Order.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<table>
    <tr>
        <th class="wide">Product</th>
        <th class="thin">Size</th>
        <th class="thin number">Quantity</th>
        <th class="thin number">Unit Price</th>
        <th class="thin number">Total Price</th>
        <th class="thin number">Delete</th>
    </tr>
    
    <% foreach(var orderItem in ViewData.Order.OrderItems) { %>
    
    <tr>
        <td><%= Html.ActionLink<ProductController>(c => c.Item(orderItem.Size.ProductId), orderItem.Size.Product.Name) %></td>
        <td><%= orderItem.Size.Name %></td>
        <td class="number"><%= orderItem.Quantity %></td>
        <td class="number"><%= orderItem.Size.Product.Price.ToString("£0.00") %></td>
        <td class="number"><%= orderItem.Total.ToString("£0.00")%></td>
        <td class="number"><%= Html.ActionLink<OrderController>(c => c.Remove(orderItem.OrderItemId), "X") %></td>
    </tr>
    
    <% } %>
    
    <tr class="total">
        <td>Total</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td class="number"><%= ViewData.Order.Total.ToString("£0.00") %></td>
        <td>&nbsp;</td>
    </tr>
    
</table>

</asp:Content>
