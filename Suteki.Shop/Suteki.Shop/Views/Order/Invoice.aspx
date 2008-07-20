﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Print.Master" AutoEventWireup="true" CodeBehind="Invoice.aspx.cs" Inherits="Suteki.Shop.Views.Order.Invoice" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>Invoice</h1>

<table>
    <tr>
        <th class="wide">Product</th>
        <th class="thin">Size</th>
        <th class="thin number">Quantity</th>
        <th class="thin number">Unit Price</th>
        <th class="thin number">Total Price</th>
    </tr>
    
    <% foreach (var basketItem in ViewData.Model.Order.Basket.BasketItems)
       { %>
    
    <tr>
        <td><%= Html.ActionLink<ProductController>(c => c.Item(basketItem.Size.Product.UrlName), basketItem.Size.Product.Name)%></td>
        <td><%= basketItem.Size.Name%></td>
        <td class="number"><%= basketItem.Quantity%></td>
        <td class="number"><%= basketItem.Size.Product.Price.ToString("£0.00")%></td>
        <td class="number"><%= basketItem.Total.ToString("£0.00")%></td>
    </tr>
    
    <% } %>
    
    <tr class="total">
        <td>Total</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td class="number"><%= ViewData.Model.Order.Basket.Total.ToString("£0.00")%></td>
    </tr>

    <tr>
        <td>Postage</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td class="number"><%= ViewData.Model.Order.Basket.PostageTotal%></td>
        <td>&nbsp;</td>
    </tr>

    <tr>
        <td>(for <%= ViewData.Model.Order.Basket.Country.Name %>)</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
    </tr>

    <tr class="total">
        <td>Total With Postage</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td class="number"><%= ViewData.Model.Order.Basket.TotalWithPostage%></td>
        <td>&nbsp;</td>
    </tr>
    
</table>


<div class="invoiceLabelLeft">

    <ul>
        <li><%= ViewData.Model.Order.PostalContact.Fullname %>&nbsp;</li>
        <li><%= ViewData.Model.Order.PostalContact.Address1%>&nbsp;</li>
        <li><%= ViewData.Model.Order.PostalContact.Address2%>&nbsp;</li>
        <li><%= ViewData.Model.Order.PostalContact.Address3%>&nbsp;</li>
        <li><%= ViewData.Model.Order.PostalContact.Town%>&nbsp;</li>
        <li><%= ViewData.Model.Order.PostalContact.County%>&nbsp;</li>
        <li><%= ViewData.Model.Order.PostalContact.Postcode%>&nbsp;</li>
        <li><%= ViewData.Model.Order.PostalContact.Country.Name%>&nbsp;</li>
    </ul>

</div>

<div class="invoiceLabelRight">

    <ul>
        <li><%= ViewData.Model.Order.PostalContact.Fullname %>&nbsp;</li>
        <li><%= ViewData.Model.Order.PostalContact.Address1%>&nbsp;</li>
        <li><%= ViewData.Model.Order.PostalContact.Address2%>&nbsp;</li>
        <li><%= ViewData.Model.Order.PostalContact.Address3%>&nbsp;</li>
        <li><%= ViewData.Model.Order.PostalContact.Town%>&nbsp;</li>
        <li><%= ViewData.Model.Order.PostalContact.County%>&nbsp;</li>
        <li><%= ViewData.Model.Order.PostalContact.Postcode%>&nbsp;</li>
        <li><%= ViewData.Model.Order.PostalContact.Country.Name%>&nbsp;</li>
    </ul>

</div>

</asp:Content>
