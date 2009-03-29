<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Item.aspx.cs" Inherits="Suteki.Shop.Views.Order.Item" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Order Confirmation</h1>
    
	<% Html.RenderPartial("OrderDetails"); %>

<h3>Customer Details</h3>

<div class="columnContainer">
    <div class="contentLeftColumn">
        
        <h3>Card Holder</h3>
        
        <dl>
            <dt>First Name</dt><dd><%= Model.Order.Contact.Firstname %>&nbsp;</dd>
            <dt>Last Name</dt><dd><%= Model.Order.Contact.Lastname %>&nbsp;</dd>
            <dt>Address 1</dt><dd><%= Model.Order.Contact.Address1 %>&nbsp;</dd>
            <dt>Address 2</dt><dd><%= Model.Order.Contact.Address2 %>&nbsp;</dd>
            <dt>Address 3</dt><dd><%= Model.Order.Contact.Address3 %>&nbsp;</dd>
            <dt>Town</dt><dd><%= Model.Order.Contact.Town %>&nbsp;</dd>
            <dt>County</dt><dd><%= Model.Order.Contact.County %>&nbsp;</dd>
            <dt>Postcode</dt><dd><%= Model.Order.Contact.Postcode %>&nbsp;</dd>
            <dt>Country</dt><dd><%= Model.Order.Contact.Country.Name %>&nbsp;</dd>
            <dt>Telephone</dt><dd><%= Model.Order.Contact.Telephone %>&nbsp;</dd>
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
        
            <% if(User.IsAdministrator()) { %>

                <%= Html.ErrorBox(ViewData.Model) %>

                <% if (ViewData.Model.Card == null) { %>

                    <% using (Html.BeginForm("ShowCard", "Order", FormMethod.Post,
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

<% if (User.IsAdministrator())
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
