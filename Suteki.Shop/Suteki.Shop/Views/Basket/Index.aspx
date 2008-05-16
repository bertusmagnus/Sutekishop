<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.Basket.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>Basket</h1>

<%= Html.ErrorBox(ViewData) %>

<% if(ViewData.Basket.IsEmpty) { %>

    <p>Your basket is empty</p>

<% } else { %>

    <table>
        <tr>
            <th class="wide">Product</th>
            <th class="thin">Size</th>
            <th class="thin number">Quantity</th>
            <th class="thin number">Unit Price</th>
            <th class="thin number">Total Price</th>
            <th class="thin number">Delete</th>
        </tr>
        
        <% foreach (var basketItem in ViewData.Basket.BasketItems)
           { %>
        
        <tr>
            <td><%= Html.ActionLink<ProductController>(c => c.Item(basketItem.Size.ProductId), basketItem.Size.Product.Name)%></td>
            <td><%= basketItem.Size.Name%></td>
            <td class="number"><%= basketItem.Quantity%></td>
            <td class="number"><%= basketItem.Size.Product.Price.ToString("£0.00")%></td>
            <td class="number"><%= basketItem.Total.ToString("£0.00")%></td>
            <td class="number"><%= Html.ActionLink<BasketController>(c => c.Remove(basketItem.BasketItemId), "X")%></td>
        </tr>
        
        <% } %>
        
        <tr class="total">
            <td>Total</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td class="number"><%= ViewData.Basket.Total.ToString("£0.00") %></td>
            <td>&nbsp;</td>
        </tr>

        <tr>
            <td>Postage</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td class="number"><%= ViewData.Basket.PostageTotal %></td>
            <td>&nbsp;</td>
        </tr>

        <tr class="total">
            <td>Total With Postage</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td class="number"><%= ViewData.Basket.TotalWithPostage %></td>
            <td>&nbsp;</td>
        </tr>
        
    </table>

    <p>&nbsp;</p>

    <%= Html.ActionLink<OrderController>(c => c.Checkout(ViewData.Basket.BasketId), "Checkout", new { _class = "linkButton" })%>

<% } %>

</asp:Content>
