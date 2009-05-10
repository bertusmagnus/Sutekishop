<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>

<%@ Import Namespace="Suteki.Common.Models" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<h1>
		Checkout</h1>
	<div id="checkout">
		<%= Html.ValidationSummary() %>
		<p>
			Welcome to our secure payment page. Please check your order and fill in the information
			below to place your order. For security puposes your information will be encrypted
			and once your order has been processed any credit card information will be destroyed.</p>
		<%= Html.ErrorBox(ViewData.Model)%>
		<%= Html.MessageBox(ViewData.Model)%>
		<!-- basket view -->
		<h3>
			Order Details</h3>
		<% Html.RenderPartial("OrderDetails"); %>
		<!-- addresses -->
		<% using (Html.BeginForm<CheckoutController>(c => c.Index(null), FormMethod.Post, new { id = "mainForm" }))
	 { %>
		<h3>
			Customer Details</h3>
		<p>
			<strong id="validation-explanation">Items in the form below that are marked with a *
				are required.</strong></p>
		<% Html.RenderPartial("CustomerDetails"); %>
		<h3>
			Payment Details</h3>
		<% Html.RenderPartial("PaymentDetails"); %>
		<p>
			<%= Html.SubmitButton("submitButton", "Continue")%></p>
		<% } %>
	</div>

	<script type="text/javascript">
$(document).ready(function(){
	toggleCardHolderDetails();
	toggleCard();
	
	$('#chkUseCardHolderContact').click(function(){
		toggleCardHolderDetails()
	});
});

function toggleCardHolderDetails() {
	var useCardholderContactCheck = $("#order_usecardholdercontact")[0];
	var deliveryAddress = $("#deliveryAddress");
	toggleVisibilityWithCheckbox(useCardholderContactCheck, deliveryAddress);
}

function toggleCard() {
	var paybytelephone = $("#order_paybytelephone")[0];
	var cardDetails = $("#cardDetails");
	toggleVisibilityWithCheckbox(paybytelephone, cardDetails);
}

function toggleVisibilityWithCheckbox(checkbox, div) {
	if($(checkbox).is(':checked')){
		div.css("display","none");
	} else {
		div.css("display","block");
	}
	//Why?
	//$('#submitButton').focus();
}

	</script>

</asp:Content>
