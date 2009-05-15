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

	<%= Html.SubmitButton("submitButton", "Continue")%>
	
	<% } %>

<script type="text/javascript">

toggleCardHolderDetails();
toggleCard();

</script>

</asp:Content>
