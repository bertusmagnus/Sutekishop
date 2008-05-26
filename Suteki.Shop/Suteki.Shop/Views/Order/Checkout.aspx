<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="Suteki.Shop.Views.Order.Checkout" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<script language="javascript">

function toggleCardHolderDetails()
{
    var useCardholderContactCheck = document.getElementsByName("order.usecardholdercontact")[0];
    var deliveryAddress = document.getElementById("deliveryAddress");
    toggleVisibilityWithCheckbox(useCardholderContactCheck, deliveryAddress);
}

function toggleCard()
{
    var paybytelephone = document.getElementsByName("order.paybytelephone")[0];
    var cardDetails = document.getElementById("cardDetails");
    toggleVisibilityWithCheckbox(paybytelephone, cardDetails);
}

function toggleVisibilityWithCheckbox(checkbox, div)
{
    if(checkbox.checked)
    {
        div.style.visibility = "hidden";
    }
    else
    {
        div.style.visibility = "visible";
    }
}

</script>

    <h1>Checkout</h1>
    
    <p>Welcome to the Suteki Shop secure payment page. Please check your order and fill in the information below to place your order. For security puposes your information will be encrypted and once your order has been processed any credit card information will be destroyed.</p>
    
    <%= Html.ErrorBox(ViewData) %>
    <%= Html.MessageBox(ViewData) %>

<!-- basket view -->

    <h3>Order Details</h3>

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

<p>The default postage & package charge displayed is for UK postal deliveries. If you select a delivery address outside the UK please check this price again.</p>
 

<!-- addresses -->

<h3>Customer Details</h3>

    <% using (Html.Form("Order", "PlaceOrder"))
       { %>

        <%= Html.Hidden("order.orderid", ViewData.Order.OrderId)%>
        <%= Html.Hidden("order.basketid", ViewData.Order.BasketId) %>
        
        <!-- card contact -->

<div class="columnContainer">
    <div class="contentLeftColumn">
        
        <h3>Card Holder</h3>
        
        <label for="cardcontact.firstname">First Name</label>
        <%= Html.TextBox("cardcontact.firstname", ViewData.Order.Contact.Firstname)%>
        
        <label for="cardcontact.lastname">Last Name</label>
        <%= Html.TextBox("cardcontact.lastname", ViewData.Order.Contact.Lastname)%>
        
        <label for="cardcontact.address1">Address</label>
        <%= Html.TextBox("cardcontact.address1", ViewData.Order.Contact.Address1)%>
        
        <label for="cardcontact.address2">&nbsp;</label>
        <%= Html.TextBox("cardcontact.address2", ViewData.Order.Contact.Address2)%>
        
        <label for="cardcontact.address3">&nbsp;</label>
        <%= Html.TextBox("cardcontact.address3", ViewData.Order.Contact.Address3)%>
        
        <label for="cardcontact.town">Town / City</label>
        <%= Html.TextBox("cardcontact.town", ViewData.Order.Contact.Town)%>

        <label for="cardcontact.county">County</label>
        <%= Html.TextBox("cardcontact.county", ViewData.Order.Contact.County)%>
 
        <label for="cardcontact.postcode">Postcode</label>
        <%= Html.TextBox("cardcontact.postcode", ViewData.Order.Contact.Postcode)%>
 
        <label for="cardcontact.countryid">Country</label>
        <%= Html.Select("cardcontact.countryid", ViewData.Countries, "Name", "CountryId", ViewData.Order.Contact.CountryId)%>
        
        <label for="cardcontact.telephone">Telephone</label>
        <%= Html.TextBox("cardcontact.telephone", ViewData.Order.Contact.Telephone)%>
 
        <!-- email details -->
        
        <label for="order.email">Email</label>
        <%= Html.TextBox("order.email", ViewData.Order.Email)%>
 
        <label for="emailconfirm">Confirm Email</label>
        <%= Html.TextBox("emailconfirm", ViewData.Order.Email)%>
 
    </div>
    <div class="contentRightColumn">
 
        <!-- deliver contact -->
        
        <h3>Delivery Address</h3>
        
        <label for="order.usecardholdercontact">Use Cardholder Details</label>
        <%= Html.CheckBox("order.usecardholdercontact", "", "True", ViewData.Order.UseCardHolderContact,
                        new { onclick = "javascript:toggleCardHolderDetails();" })%>
        
        <div id="deliveryAddress">
        
            <label for="deliverycontact.firstname">First Name</label>
            <%= Html.TextBox("deliverycontact.firstname", ViewData.Order.Contact1.Firstname)%>
            
            <label for="deliverycontact.lastname">Last Name</label>
            <%= Html.TextBox("deliverycontact.lastname", ViewData.Order.Contact1.Lastname)%>
            
            <label for="deliverycontact.address1">Address</label>
            <%= Html.TextBox("deliverycontact.address1", ViewData.Order.Contact1.Address1)%>
            
            <label for="deliverycontact.address2">&nbsp;</label>
            <%= Html.TextBox("deliverycontact.address2", ViewData.Order.Contact1.Address2)%>
            
            <label for="deliverycontact.address3">&nbsp;</label>
            <%= Html.TextBox("deliverycontact.address3", ViewData.Order.Contact1.Address3)%>
            
            <label for="deliverycontact.town">Town / City</label>
            <%= Html.TextBox("deliverycontact.town", ViewData.Order.Contact1.Town)%>

            <label for="deliverycontact.county">County</label>
            <%= Html.TextBox("deliverycontact.county", ViewData.Order.Contact1.County)%>
     
            <label for="deliverycontact.postcode">Postcode</label>
            <%= Html.TextBox("deliverycontact.postcode", ViewData.Order.Contact1.Postcode)%>
     
            <label for="deliverycontact.countryid">Country</label>
            <%= Html.Select("deliverycontact.countryid", ViewData.Countries, "Name", "CountryId", ViewData.Order.Contact1.CountryId)%>
            
            <label for="deliverycontact.telephone">Telephone</label>
            <%= Html.TextBox("deliverycontact.telephone", ViewData.Order.Contact1.Telephone)%>

        </div>
        
        <!-- additional information -->  
        
        <label for="order.additionalinformation">Additional Information</label>
        <%= Html.TextArea("order.additionalinformation", ViewData.Order.AdditionalInformation)%>
        
    </div>      
</div>        
         
        <h3>Payment Details</h3>     
        
<div class="columnContainer">
    <div class="contentLeftColumn">
        
        <div id="cardDetails">
        
            <label for="card.cardtypeid">Country</label>
            <%= Html.Select("card.cardtypeid", ViewData.CardTypes, "Name", "CardTypeId", ViewData.Order.Card.CardTypeId)%>
            
            <label for="card.holder">Card Holder</label>
            <%= Html.TextBox("card.holder", ViewData.Order.Card.Holder)%>
     
            <label for="card.number">Card Number</label>
            <%= Html.TextBox("card.number", ViewData.Order.Card.Number)%>
     
            <div class="cardDate">
                <label for="card.expirymonth">Expire Date</label>
                <%= Html.Select("card.expirymonth", Card.Months.Select(m => new { Value = m }), "Value", "Value", ViewData.Order.Card.ExpiryMonth)%>
                <%= Html.Select("card.expiryyear", Card.ExpiryYears.Select(m => new { Value = m }), "Value", "Value", ViewData.Order.Card.ExpiryYear)%>
            </div>
     
            <div class="cardDate">
                <label for="card.startmonth">Start Data</label>
                <%= Html.Select("card.startmonth", Card.Months.Select(m => new { Value = m }), "Value", "Value", ViewData.Order.Card.StartMonth)%>
                <%= Html.Select("card.startyear", Card.StartYears.Select(m => new { Value = m }), "Value", "Value", ViewData.Order.Card.StartYear)%>
            </div>
     
            <label for="card.issuenumber">Issue Number</label>
            <%= Html.TextBox("card.issuenumber", ViewData.Order.Card.IssueNumber)%>
     
            <label for="card.securitycode">Security Code</label>
            <%= Html.TextBox("card.securitycode", ViewData.Order.Card.SecurityCode)%>

        </div>
    </div>
    <div class="contentRightColumn">
    
        <label for="order.paybytelephone"><strong>I prefer to pay by cheque or telephone</strong></label>
        <%= Html.CheckBox("order.paybytelephone", "", "True", ViewData.Order.PayByTelephone,
                        new { onclick = "javascript:toggleCard();" })%>
    
        <p>If you tick this option you will receive an order number.</p>
        <p>You should quote this number when you contact us with your payment. We accept most major credit and debit cards including Amex.</p>
        <p>We also accept cheques( in pounds sterling from a UK bank only) and postal orders made payable to ‘Jump the Gun’ Please note that cheques will have to await clearance before we can dispatch goods- this can take six working days.</p>
        <p>Unfortunately we are unable to accept credit card orders from Indonesia due to high levels of fraud- apologies to our customers in that part of the world.</p>
        <p>All credit card details are collected using an SSL secure server which means that any sensitive information is securely encrypted and cannot be read by any unauthorised party. We do not store any of your credit card details once payment has been taken. We will not take payment until your goods are ready for dispatch.</p>
    </div>
</div> 
<div class="clear" />       

        <%= Html.SubmitButton("submit", "Place Order")%>

    <% } %>


<script>

toggleCardHolderDetails();
toggleCard();

</script>

</asp:Content>
