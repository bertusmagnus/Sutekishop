<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Checkout.aspx.cs" Inherits="Suteki.Shop.Views.Order.Checkout" %>
<%@ Import Namespace="Suteki.Common.Models"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<script language="javascript">

var first = true;

function toggleCardHolderDetails()
{
    var useCardholderContactCheck = document.getElementsByName("order.usecardholdercontact")[0];
    var deliveryAddress = document.getElementById("deliveryAddress");
    toggleVisibilityWithCheckbox(useCardholderContactCheck, deliveryAddress);
    
    updatePostageOnUseCardholderDetailsChange(useCardholderContactCheck);
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

function updatePostageOnUseCardholderDetailsChange(checkbox)
{
    if(first)
    {
        first = false;
        return;
    }

    var select;
    if(checkbox.checked)
    {
        select = document.getElementById("cardcontact.countryid");
    }
    else
    {
        select = document.getElementById("deliverycontact.countryid");
    }
    updateSelectedCountry(select);
}

function updateSelectedCountry(select)
{
    var useCardholderContactCheck = document.getElementsByName("order.usecardholdercontact")[0];
    
    if((!useCardholderContactCheck.checked && select.id) == "cardcontact.countryid") return;
    
    for(var i = 0; i < select.options.length; i++)
    {
        if(select.options[i].selected)
        {
            alert("Postage will be updated for " + select.options[i].text);
            
            var form = document.getElementById("mainForm");
            
            var url = <%= "\"" + Url.RouteUrl(new { Controller = "Order", Action = "UpdateCountry", Id = ViewData.Model.Order.Basket.BasketId }) + "\"" %>
                 + "?countryId=" + select.options[i].value ;
                 
            form.action = url;
            form.submit();
        }
    }
}

function addHandlers()
{
    var cardcontactCountryid = document.getElementById("cardcontact.countryid");
    cardcontactCountryid.onchange = function() { updateSelectedCountry(this); }
    
    var deliverycontactCountryid = document.getElementById("deliverycontact.countryid");
    deliverycontactCountryid.onchange = function() { updateSelectedCountry(this); }
}

</script>

    <h1>Checkout</h1>
    
    <p>Welcome to our secure payment page. Please check your order and fill in the information below to place your order. For security puposes your information will be encrypted and once your order has been processed any credit card information will be destroyed.</p>
    
    <%= Html.ErrorBox(ViewData.Model)%>
    <%= Html.MessageBox(ViewData.Model)%>

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
        <td>(for <%= ViewData.Model.Order.Basket.Country.Name%>)</td>
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

<p>The default postage & package charge displayed is for UK postal deliveries. If you select a delivery address outside the UK please check this price again.</p>
 

<!-- addresses -->

<h3>Customer Details</h3>

    <form method="post" action="<%= Url.Action("PlaceOrder", "Order").ToSslUrl() %>" id="mainForm" name="mainForm">

        <%= Html.Hidden("order.orderid", ViewData.Model.Order.OrderId.ToString())%>
        <%= Html.Hidden("order.basketid", ViewData.Model.Order.BasketId.ToString())%>
        
        <!-- card contact -->

<div class="columnContainer">
    <div class="contentLeftColumn">
        
        <h3>Card Holder</h3>
        
        <table>
            <tr>
                <td class="label"><label for="cardcontact.firstname">First Name</label></td>
                <td class="field"><%= Html.TextBox("cardcontact.firstname", ViewData.Model.Order.Contact.Firstname)%></td>
            </tr>
            <tr>
                <td class="label"><label for="cardcontact.lastname">Last Name</label></td>
                <td class="field"><%= Html.TextBox("cardcontact.lastname", ViewData.Model.Order.Contact.Lastname)%></td>
            </tr>
            <tr>
                <td class="label"><label for="cardcontact.address1">Address</label></td>
                <td class="field"><%= Html.TextBox("cardcontact.address1", ViewData.Model.Order.Contact.Address1)%></td>
            </tr>
            <tr>
                <td class="label"><label for="cardcontact.address2">&nbsp;</label></td>
                <td class="field"><%= Html.TextBox("cardcontact.address2", ViewData.Model.Order.Contact.Address2)%></td>
            </tr>
            <tr>
                <td class="label"><label for="cardcontact.address3">&nbsp;</label></td>
                <td class="field"><%= Html.TextBox("cardcontact.address3", ViewData.Model.Order.Contact.Address3)%></td>
            </tr>
            <tr>
                <td class="label"><label for="cardcontact.town">Town / City</label></td>
                <td class="field"><%= Html.TextBox("cardcontact.town", ViewData.Model.Order.Contact.Town)%></td>
            </tr>
            <tr>
                <td class="label"><label for="cardcontact.county">County</label></td>
                <td class="field"><%= Html.TextBox("cardcontact.county", ViewData.Model.Order.Contact.County)%></td>
            </tr>
            <tr>
                <td class="label"><label for="cardcontact.postcode">Postcode</label></td>
                <td class="field"><%= Html.TextBox("cardcontact.postcode", ViewData.Model.Order.Contact.Postcode)%></td>
            </tr>
            <tr>
                <td class="label"><label for="cardcontact.countryid">Country</label></td>
                <td class="field"><%= Html.DropDownList("cardcontact.countryid", new SelectList(ViewData.Model.Countries, "CountryId", "Name", ViewData.Model.Order.CardContactCountryId))%></td>
            </tr>
            <tr>
                <td class="label"><label for="cardcontact.telephone">Telephone</label></td>
                <td class="field"><%= Html.TextBox("cardcontact.telephone", ViewData.Model.Order.Contact.Telephone)%></td>
            </tr>
            <tr>
                <td class="label"><label for="order.email">Email</label></td>
                <td class="field"><%= Html.TextBox("order.email", ViewData.Model.Order.Email)%></td>
            </tr>
            <tr>
                <td class="label"><label for="emailconfirm">Confirm Email</label></td>
                <td class="field"><%= Html.TextBox("emailconfirm", ViewData.Model.Order.Email)%></td>
            </tr>
        </table>
        
    </div>
    <div class="contentRightColumn">
 
        <!-- deliver contact -->
        
        <h3>Delivery Address</h3>
        
        <table>
            <tr>
                <td class="label"><label for="order.usecardholdercontact">Use Cardholder Details</label></td>
                <td class="field"><%= Html.CheckBox("order.usecardholdercontact", ViewData.Model.Order.UseCardHolderContact,
                        new { onclick = "javascript:toggleCardHolderDetails();" })%></td>
            </tr>
        </table>
        
        <div id="deliveryAddress">

            <table>
                <tr>
                    <td class="label"><label for="deliverycontact.firstname">First Name</label></td>
                    <td class="field"><%= Html.TextBox("deliverycontact.firstname", ViewData.Model.Order.Contact1.Firstname)%></td>
                </tr>
                <tr>
                    <td class="label"><label for="deliverycontact.lastname">Last Name</label></td>
                    <td class="field"><%= Html.TextBox("deliverycontact.lastname", ViewData.Model.Order.Contact1.Lastname)%></td>
                </tr>
                <tr>
                    <td class="label"><label for="deliverycontact.address1">Address</label></td>
                    <td class="field"><%= Html.TextBox("deliverycontact.address1", ViewData.Model.Order.Contact1.Address1)%></td>
                </tr>
                <tr>
                    <td class="label"><label for="deliverycontact.address2">&nbsp;</label></td>
                    <td class="field"><%= Html.TextBox("deliverycontact.address2", ViewData.Model.Order.Contact1.Address2)%></td>
                </tr>
                <tr>
                    <td class="label"><label for="deliverycontact.address3">&nbsp;</label></td>
                    <td class="field"><%= Html.TextBox("deliverycontact.address3", ViewData.Model.Order.Contact1.Address3)%></td>
                </tr>
                <tr>
                    <td class="label"><label for="deliverycontact.town">Town / City</label></td>
                    <td class="field"><%= Html.TextBox("deliverycontact.town", ViewData.Model.Order.Contact1.Town)%></td>
                </tr>
                <tr>
                    <td class="label"><label for="deliverycontact.county">County</label></td>
                    <td class="field"><%= Html.TextBox("deliverycontact.county", ViewData.Model.Order.Contact1.County)%></td>
                </tr>
                <tr>
                    <td class="label"><label for="deliverycontact.postcode">Postcode</label></td>
                    <td class="field"><%= Html.TextBox("deliverycontact.postcode", ViewData.Model.Order.Contact1.Postcode)%></td>
                </tr>
                <tr>
                    <td class="label"><label for="deliverycontact.countryid">Country</label></td>
                    <td class="field"><%= Html.DropDownList("deliverycontact.countryid", new SelectList(ViewData.Model.Countries, "CountryId", "Name", ViewData.Model.Order.DeliveryContactCountryId))%></td>
                </tr>
                <tr>
                    <td class="label"><label for="deliverycontact.telephone">Telephone</label></td>
                    <td class="field"><%= Html.TextBox("deliverycontact.telephone", ViewData.Model.Order.Contact1.Telephone)%></td>
                </tr>
            </table>
        
        </div>
        
        <!-- additional information -->  
        
        <table>
            <tr>
                <td class="label"><label for="order.additionalinformation">Additional Information</label></td>
                <td class="field"><span><%= Html.TextArea("order.additionalinformation", ViewData.Model.Order.AdditionalInformation)%></span></td>
            </tr>
        </table>
        
    </div>      
</div>        
         
        <h3>Payment Details</h3>     
        
<div class="columnContainer">
    <div class="contentLeftColumn">
        
        <div id="cardDetails">
        
            <table>
                <tr>
                    <td class="label"><label for="card.cardtypeid">Card Type</label></td>
                    <td colspan="2" class="field"><%= Html.DropDownList("card.cardtypeid", new SelectList(ViewData.Model.CardTypes, "CardTypeId", "Name", ViewData.Model.Order.Card.CardTypeId))%></td>
                </tr>
                <tr>
                    <td class="label"><label for="card.holder">Card Holder</label></td>
                    <td colspan="2" class="field"><%= Html.TextBox("card.holder", ViewData.Model.Order.Card.Holder)%></td>
                </tr>
                <tr>
                    <td class="label"><label for="card.number">Card Number</label></td>
                    <td colspan="2" class="field"><%= Html.TextBox("card.number")%></td>
                </tr>
                <tr>
                    <td class="label"><label for="card.expirymonth">Expire Date</label></td>
                    <td class="field small"><%= Html.DropDownList("card.expirymonth", Card.Months.ToStringValues().AsSelectList(ViewData.Model.Order.Card.ExpiryMonth))%></td>
                    <td class="field small"><%= Html.DropDownList("card.expiryyear", Card.ExpiryYears.ToStringValues().AsSelectList(ViewData.Model.Order.Card.ExpiryYear))%></td>
                </tr>
                <tr>
                    <td class="label"><label for="card.startmonth">Start Date</label><br />If present on your card</td>
                    <td class="field small"><%= Html.DropDownList("card.startmonth", Card.Months.ToStringValues().AddBlankFirstValue().AsSelectList(ViewData.Model.Order.Card.StartMonth))%></td>
                    <td class="field small"><%= Html.DropDownList("card.startyear", Card.StartYears.ToStringValues().AddBlankFirstValue().AsSelectList(ViewData.Model.Order.Card.StartYear))%></td>
                </tr>
                <tr>
                    <td class="label"><label for="card.issuenumber">Issue Number</label><br />If present on your card</td>
                    <td colspan="2" class="field small"><%= Html.TextBox("card.issuenumber", ViewData.Model.Order.Card.IssueNumber, new { maxlength = "1" })%></td>
                </tr>
                <tr>
                    <td class="label"><label for="card.securitycode">Security Code</label></td>
                    <td colspan="2" class="field small"><%= Html.TextBox("card.securitycode", new { maxlength = "4" })%></td>
                </tr>
                <tr>
                    <td colspan="3"><p>The last three digits printed on the signature strip of your credit/debit card. Or for Amex. the four digits printed on the front of the card above the embossed card number.</p></td>
                </tr>
            </table>
        </div>
    </div>
    <div class="contentRightColumn">
    
        <label for="order.paybytelephone"><strong>I prefer to pay by cheque or telephone</strong></label>
        <%= Html.CheckBox("order.paybytelephone", ViewData.Model.Order.PayByTelephone,
                        new { onclick = "javascript:toggleCard();" })%>
    
        <p>If you tick this option you will receive an order number. You should quote this number when you contact us with your payment. We accept most major credit and debit cards including Amex. We also accept cheques( in pounds sterling from a UK bank only) and postal orders made payable to ‘Jump the Gun’ Please note that cheques will have to await clearance before we can dispatch goods- this can take six working days.</p>
        <p>All credit card details are collected using an SSL secure server which means that any sensitive information is securely encrypted and cannot be read by any unauthorised party. We do not store any of your credit card details once payment has been taken. We will not take payment until your goods are ready for dispatch.</p>
    </div>
</div> 
<div class="clear" />       

        <%= Html.SubmitButton("submitButton", "Place Order")%>

    </form>


<script>

toggleCardHolderDetails();
toggleCard();
addHandlers();

</script>

</asp:Content>
