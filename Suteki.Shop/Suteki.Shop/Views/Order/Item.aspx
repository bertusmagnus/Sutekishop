<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Item.aspx.cs" Inherits="Suteki.Shop.Views.Order.Item" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Order Confirmation</h1>
    
    <h3>Order Details</h3>

<dl>
    <dt>Order Number</dt><dd><%= ViewData.Order.OrderId.ToString() %>&nbsp;</dd>
    <dt>Date</dt><dd><%= ViewData.Order.CreatedDate.ToShortDateString() %></dd>

<% if(User.IsInRole("Administrator")) { %>
    <dt>Status</dt><dd><%= ViewData.Order.OrderStatus.Name %></dd>
    <dt>Dispatched</dt><dd><%= ViewData.Order.DispatchedDateAsString %></dd>
    <dt>Updated by</dt><dd><%= ViewData.Order.UserAsString %></dd>
<% } %>
    
</dl>

<% if(User.IsInRole("Administrator") && ViewData.Order.IsCreated) { %>
<div class="orderAction">
    <%= Html.ActionLink<OrderController>(c => c.Dispatch(ViewData.Order.OrderId), "Dispatch", new { _class = "linkButton" })%>
    <%= Html.ActionLink<OrderController>(c => c.Reject(ViewData.Order.OrderId), "Reject", new { _class = "linkButton" })%>
</div>    
<% } %>

<table>
    <tr>
        <th class="wide">Product</th>
        <th class="thin">Size</th>
        <th class="thin number">Quantity</th>
        <th class="thin number">Unit Price</th>
        <th class="thin number">Total Price</th>
    </tr>
    
    <% foreach (var basketItem in ViewData.Order.Basket.BasketItems)
       { %>
    
    <tr>
        <td><%= Html.ActionLink<ProductController>(c => c.Item(basketItem.Size.ProductId), basketItem.Size.Product.Name)%></td>
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
        <td class="number"><%= ViewData.Order.Basket.Total.ToString("£0.00")%></td>
    </tr>

    <tr>
        <td>Postage</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td class="number"><%= ViewData.Order.Basket.PostageTotal%></td>
        <td>&nbsp;</td>
    </tr>

    <tr class="total">
        <td>Total With Postage</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td class="number"><%= ViewData.Order.Basket.TotalWithPostage%></td>
        <td>&nbsp;</td>
    </tr>
    
</table>

<h3>Customer Details</h3>

<div class="columnContainer">
    <div class="contentLeftColumn">
        
        <h3>Card Holder</h3>
        
        <dl>
            <dt>First Name</dt><dd><%= ViewData.Order.Contact.Firstname %>&nbsp;</dd>
            <dt>Last Name</dt><dd><%= ViewData.Order.Contact.Lastname %>&nbsp;</dd>
            <dt>Address 1</dt><dd><%= ViewData.Order.Contact.Address1 %>&nbsp;</dd>
            <dt>Address 2</dt><dd><%= ViewData.Order.Contact.Address2 %>&nbsp;</dd>
            <dt>Address 3</dt><dd><%= ViewData.Order.Contact.Address3 %>&nbsp;</dd>
            <dt>Town</dt><dd><%= ViewData.Order.Contact.Town %>&nbsp;</dd>
            <dt>County</dt><dd><%= ViewData.Order.Contact.County %>&nbsp;</dd>
            <dt>Postcode</dt><dd><%= ViewData.Order.Contact.Postcode %>&nbsp;</dd>
            <dt>Country</dt><dd><%= ViewData.Order.Contact.Country.Name %>&nbsp;</dd>
            <dt>Telephone</dt><dd><%= ViewData.Order.Contact.Telephone %>&nbsp;</dd>
            <dt>Email</dt><dd><%= ViewData.Order.Email %>&nbsp;</dd>
        </dl>
        
    </div>
    <div class="contentRightColumn">
        <!-- deliver contact -->
        
        <h3>Delivery Address</h3>
        
        <% if(ViewData.Order.UseCardHolderContact) { %>
        
        <p>Use Card Holder Contact</p>
        
        <% } else { %>
        
        <dl>
            <dt>First Name</dt><dd><%= ViewData.Order.Contact1.Firstname %>&nbsp;</dd>
            <dt>Last Name</dt><dd><%= ViewData.Order.Contact1.Lastname %>&nbsp;</dd>
            <dt>Address 1</dt><dd><%= ViewData.Order.Contact1.Address1 %>&nbsp;</dd>
            <dt>Address 2</dt><dd><%= ViewData.Order.Contact1.Address2 %>&nbsp;</dd>
            <dt>Address 3</dt><dd><%= ViewData.Order.Contact1.Address3 %>&nbsp;</dd>
            <dt>Town</dt><dd><%= ViewData.Order.Contact1.Town %>&nbsp;</dd>
            <dt>County</dt><dd><%= ViewData.Order.Contact1.County %>&nbsp;</dd>
            <dt>Postcode</dt><dd><%= ViewData.Order.Contact1.Postcode %>&nbsp;</dd>
            <dt>Country</dt><dd><%= ViewData.Order.Contact1.Country.Name %>&nbsp;</dd>
            <dt>Telephone</dt><dd><%= ViewData.Order.Contact1.Telephone %>&nbsp;</dd>
            <dt>Additional Information</dt><dd><%= ViewData.Order.AdditionalInformation %>&nbsp;</dd>
        </dl>
        
        <% } %>
        
    </div>      
</div>        
         
        <h3>Payment Details</h3>     
        
<div class="columnContainer">
    <div class="contentLeftColumn">

        <% if(ViewData.Order.PayByTelephone) { %>
        
        <p>Pay By Telephone</p>
        
        <% } else { %>

        <dl>
            <dt>Card Type</dt><dd><%= ViewData.Order.Card.CardType.Name %>&nbsp;</dd>
            <dt>Card Holder</dt><dd><%= ViewData.Order.Card.Holder %>&nbsp;</dd>
            <dt>Card Number</dt><dd>xxxx-xxxx-xxxx-xxxx</dd>
        </dl>
        
        <% } %>
        
    </div>
    <div class="contentRightColumn">

    </div>
</div>

</asp:Content>
