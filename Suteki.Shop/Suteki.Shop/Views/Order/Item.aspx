<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Item.aspx.cs" Inherits="Suteki.Shop.Views.Order.Item" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Order Confirmation</h1>
    
    <h3>Order Details</h3>

    <p><%= Html.ActionLink<OrderController>(c => c.Print(ViewData.Model.Order.OrderId), "Printable version of this page") %></p>
<dl>
    <dt>Order Number</dt><dd><%= ViewData.Model.Order.OrderId.ToString()%>&nbsp;</dd>
    <dt>Date</dt><dd><%= ViewData.Model.Order.CreatedDate.ToShortDateString()%></dd>

<% if(User.IsInRole("Administrator")) { %>
    <dt>Status</dt><dd><%= ViewData.Model.Order.OrderStatus.Name %> <%= ViewData.Model.Order.DispatchedDateAsString %></dd>
    <dt>Updated by</dt><dd><%= ViewData.Model.Order.UserAsString %></dd>
<% } %>
    
</dl>

<div class="orderAction">
<% if(User.IsInRole("Administrator")) { %>
    <% if(ViewData.Model.Order.IsCreated) { %>
        <%= Html.ActionLink<OrderController>(c => c.Dispatch(ViewData.Model.Order.OrderId), "Dispatch", new { _class = "linkButton" })%>
        <%= Html.ActionLink<OrderController>(c => c.Reject(ViewData.Model.Order.OrderId), "Reject", new { _class = "linkButton" })%>
    <% } %>
    <%= Html.ActionLink<OrderController>(c => c.Invoice(ViewData.Model.Order.OrderId), "Print Invoice", new { _class = "linkButton" }) %>
<% } %>
</div>    

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

<h3>Customer Details</h3>

<div class="columnContainer">
    <div class="contentLeftColumn">
        
        <h3>Card Holder</h3>
        
        <dl>
            <dt>First Name</dt><dd><%= ViewData.Model.Order.Contact.Firstname %>&nbsp;</dd>
            <dt>Last Name</dt><dd><%= ViewData.Model.Order.Contact.Lastname %>&nbsp;</dd>
            <dt>Address 1</dt><dd><%= ViewData.Model.Order.Contact.Address1 %>&nbsp;</dd>
            <dt>Address 2</dt><dd><%= ViewData.Model.Order.Contact.Address2 %>&nbsp;</dd>
            <dt>Address 3</dt><dd><%= ViewData.Model.Order.Contact.Address3 %>&nbsp;</dd>
            <dt>Town</dt><dd><%= ViewData.Model.Order.Contact.Town %>&nbsp;</dd>
            <dt>County</dt><dd><%= ViewData.Model.Order.Contact.County %>&nbsp;</dd>
            <dt>Postcode</dt><dd><%= ViewData.Model.Order.Contact.Postcode %>&nbsp;</dd>
            <dt>Country</dt><dd><%= ViewData.Model.Order.Contact.Country.Name %>&nbsp;</dd>
            <dt>Telephone</dt><dd><%= ViewData.Model.Order.Contact.Telephone %>&nbsp;</dd>
            <dt>Email</dt><dd><%= Html.Mailto(ViewData.Model.Order.Email, ViewData.Model.Order.Email) %>&nbsp;</dd>
        </dl>
        
    </div>
    <div class="contentRightColumn">
        <!-- deliver contact -->
        
        <h3>Delivery Address</h3>
        
        <% if(ViewData.Model.Order.UseCardHolderContact) { %>
        
        <p>Use Card Holder Contact</p>
        
        <% } else { %>
        
        <dl>
            <dt>First Name</dt><dd><%= ViewData.Model.Order.Contact1.Firstname %>&nbsp;</dd>
            <dt>Last Name</dt><dd><%= ViewData.Model.Order.Contact1.Lastname %>&nbsp;</dd>
            <dt>Address 1</dt><dd><%= ViewData.Model.Order.Contact1.Address1 %>&nbsp;</dd>
            <dt>Address 2</dt><dd><%= ViewData.Model.Order.Contact1.Address2 %>&nbsp;</dd>
            <dt>Address 3</dt><dd><%= ViewData.Model.Order.Contact1.Address3 %>&nbsp;</dd>
            <dt>Town</dt><dd><%= ViewData.Model.Order.Contact1.Town %>&nbsp;</dd>
            <dt>County</dt><dd><%= ViewData.Model.Order.Contact1.County %>&nbsp;</dd>
            <dt>Postcode</dt><dd><%= ViewData.Model.Order.Contact1.Postcode %>&nbsp;</dd>
            <dt>Country</dt><dd><%= ViewData.Model.Order.Contact1.Country.Name %>&nbsp;</dd>
            <dt>Telephone</dt><dd><%= ViewData.Model.Order.Contact1.Telephone %>&nbsp;</dd>
        </dl>
        
        <% } %>
        
        <dl>
            <dt>Additional Information</dt><dd><%= ViewData.Model.Order.AdditionalInformation %>&nbsp;</dd>
        </dl>
        
    </div>      
</div>        
<div class="clear"></div>         

<h3>Payment Details</h3>     
        
<div class="columnContainer">
    <div class="contentLeftColumn">

        <% if(ViewData.Model.Order.PayByTelephone) { %>
        
        <p>Pay By Telephone</p>
        
        <% } else { %>

        <dl>
            <dt>Card Type</dt><dd><%= ViewData.Model.Order.Card.CardType.Name %>&nbsp;</dd>
            <dt>Card Holder</dt><dd><%= ViewData.Model.Order.Card.Holder %>&nbsp;</dd>
        </dl>
        
            <% if(User.IsInRole("Administrator")) { %>

                <%= Html.ErrorBox(ViewData.Model) %>

                <% if (ViewData.Model.Card == null) { %>

                    <% using (Html.Form("Order", "ShowCard", FormMethod.Post,
                           new Dictionary<string, object> { { "onsubmit", "submitHandler();" } }))
                       { %>
                        
                        <%= Html.Hidden("orderId", ViewData.Model.Order.OrderId.ToString()) %>
                        
                        <label for="privateKey">Private Key</label>
                        <%= Html.TextBox("privateKey")%>
                        
                        <%= Html.SubmitButton("cardDetailsSubmit", "Get Card Details")%>

                    <% } %>
                
                <% } else { %>
                
                    <dl>
                        <dt>Card Number</dt><dd><%= ViewData.Model.Card.Number %></dd>
                        <dt>Issue Number</dt><dd><%= ViewData.Model.Card.IssueNumber %></dd>
                        <dt>Security Code</dt><dd><%= ViewData.Model.Card.SecurityCode %></dd>
                        <dt>Start Date</dt><dd><%= ViewData.Model.Card.StartDateAsString %></dd>
                        <dt>Expiry Date</dt><dd><%= ViewData.Model.Card.ExpiryDateAsString %></dd>
                    </dl>
                
                <% } %>

            <% } %>        
        
        <% } %>
        
    </div>
    <div class="contentRightColumn">

    </div>
</div>

<% if (User.IsInRole("Administrator"))
   { %>

    <script type="text/javascript">
    
        init();
    
        function submitHandler()
        {
            var text = document.getElementById("privateKey");
            setCookie("privateKey", text.value);
        }
        
        function init()
        {
            var text = document.getElementById("privateKey");
            if(text)
            {
                var value = getCookie("privateKey");
                if(value)
                {
                    text.value = value;
                }
            }
        }
        
        function setCookie(name, value, expires, path, domain, secure) 
        {
            var curCookie = name + "=" + escape(value) +
                ((expires) ? "; expires=" + expires.toGMTString() : "") +
                ((path) ? "; path=" + path : "") +
                ((domain) ? "; domain=" + domain : "") +
                ((secure) ? "; secure" : "");
            document.cookie = curCookie;
        }        
        
        function getCookie(name) 
        {
            var dc = document.cookie;
            var prefix = name + "=";
            var begin = dc.indexOf("; " + prefix);
            if (begin == -1) 
            {
                begin = dc.indexOf(prefix);
                if (begin != 0) return null;
            } 
            else
            {
                begin += 2;
            }
            
            var end = document.cookie.indexOf(";", begin);
            if (end == -1)
            end = dc.length;
            return unescape(dc.substring(begin + prefix.length, end));
        }        

    </script>

<% } %>

</asp:Content>
