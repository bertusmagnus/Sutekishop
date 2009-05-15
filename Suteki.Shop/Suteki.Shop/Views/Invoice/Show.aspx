<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Print.Master" Inherits="System.Web.Mvc.ViewPage<ShopViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<div class="invoiceHeader">
    <img src="http://static.jumpthegun.co.uk.s3.amazonaws.com/invoice_header.png" />
</div>

<table class="invoiceTable">
    <tr>
        <td>
            LB International Ltd,<br />
            28a North Road,<br />
            Brighton,<br />
            BN1 1YB,<br />
            UK.<br /><br />
            Tel: 01273 626 333,<br /><br />  
            info@jumpthegun.co.uk<br />
        </td>
        <td>  
            <span class="invoiceNumber">Invoice No. <%= ViewData.Model.Order.OrderId.ToString() %></span> <br /><br />
            <%= DateTime.Now.ToLongDateString() %><br /><br />
            <% foreach(var line in ViewData.Model.Order.Contact.GetAddressLines()) { %>
                <%= line %><br />
            <% } %>
        </td>
    </tr>
</table>

<div class="invoiceBasket">
<table>
    <tr>
        <th class="wide">Product</th>
        <th class="wide number">Size</th>
        <th class="wide number">Quantity</th>
        <th class="wide number">Unit Price</th>
        <th class="wide number">Total Price</th>
    </tr>
    
    <tr></tr>
    
    <% foreach (var basketItem in ViewData.Model.Order.Basket.BasketItems)
       { %>
    
    <tr>
        <td><%= Html.ActionLink<ProductController>(c => c.Item(basketItem.Size.Product.UrlName), basketItem.Size.Product.Name)%></td>
        <td class="number"><%= basketItem.Size.Name%></td>
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
<div class="invoiceThanks">Paid with Thanks</div>
</div>

<p>Registered in England No. 2341339 Vat No: 505 0466 78</p>

<p>Please use the sticker below if you need to send us anything. (stamps are neccessary).</p>

<div class="invoiceLabelLeft">

    <ul>
        <td>            
            <% foreach(var line in ViewData.Model.Order.PostalContact.GetAddressLines()) { %>
                <li><%= line %>&nbsp;</li>
            <% } %>
        </td>
    </ul>

</div>

<div class="invoiceLabelRight">

    <ul>
        <li>Jump the Gun</li>
        <li>28a North Road,</li>
        <li>Brighton,</li>
        <li>BN1 1YB,</li>
        <li>UK.</li>
    </ul>

</div>

</asp:Content>
