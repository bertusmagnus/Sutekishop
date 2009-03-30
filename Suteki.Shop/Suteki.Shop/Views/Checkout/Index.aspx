<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master"  Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<%@ Import Namespace="Suteki.Common.Models"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<script type="text/javascript" language="javascript">

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
        select = document.getElementById("cardcontact_countryid");
    }
    else
    {
        select = document.getElementById("deliverycontact_countryid");
    }
    updateSelectedCountry(select);
}

function updateSelectedCountry(select)
{
    var useCardholderContactCheck = document.getElementsByName("order.usecardholdercontact")[0];
    
    if((!useCardholderContactCheck.checked && select.id) == "cardcontact_countryid") return;
    
    for(var i = 0; i < select.options.length; i++)
    {
        if(select.options[i].selected)
        {
            alert("Postage will be updated for " + select.options[i].text);
            
            var form = document.getElementById("mainForm");
            
            var url = <%= "\"" + Url.RouteUrl(new { Controller = "Checkout", Action = "UpdateCountry", Id = ViewData.Model.Order.Basket.BasketId }) + "\"" %>
                 + "?countryId=" + select.options[i].value ;
                 
            form.action = url;
            form.submit();
        }
    }
}

function addHandlers()
{
    var cardcontactCountryid = document.getElementById("cardcontact_countryid");
    cardcontactCountryid.onchange = function() { updateSelectedCountry(this); }
    
    var deliverycontactCountryid = document.getElementById("deliverycontact_countryid");
    deliverycontactCountryid.onchange = function() { updateSelectedCountry(this); }
}

</script>

    <h1>Checkout</h1>
	<%= Html.ValidationSummary() %>
    
    <p>Welcome to our secure payment page. Please check your order and fill in the information below to place your order. For security puposes your information will be encrypted and once your order has been processed any credit card information will be destroyed.</p>
    
    <%= Html.ErrorBox(ViewData.Model)%>
    <%= Html.MessageBox(ViewData.Model)%>

<!-- basket view -->

    <h3>Order Details</h3>
	<% Html.RenderPartial("OrderDetails"); %>

<!-- addresses -->
	
	<% using (Html.BeginForm<CheckoutController>(c => c.Index(null), FormMethod.Post, new { name = "mainForm", id = "mainForm" })) { %>
	
	<h3>Customer Details</h3>
	<% Html.RenderPartial("CustomerDetails"); %>

	<h3>Payment Details</h3>     
	<% Html.RenderPartial("PaymentDetails"); %>
	
	<% } %>

<script type="text/javascript">

toggleCardHolderDetails();
toggleCard();
addHandlers();

</script>

</asp:Content>
