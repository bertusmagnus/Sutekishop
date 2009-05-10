<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ShopViewData>" %>
<%@ Import Namespace="Suteki.Common.Models"%>

<div id="cardDetails">
	<p><%= Html.Image("~/content/images/creditcards.gif", "Credit Cards") %></p>
    <p>
        <label for="card.cardtypeid">Card Type</label>
        <%= Html.DropDownList("card.cardtypeid", new SelectList(ViewData.Model.CardTypes, "CardTypeId", "Name", ViewData.Model.Order.Card.CardTypeId))%>
    </p>
    <p>
        <label for="card.holder">Card Holder</label>
        <%= Html.TextBox("card.holder", ViewData.Model.Order.Card.Holder)%>
    </p>
    <p>
        <label for="card.number">Card Number</label>
        <%= Html.TextBox("card.number", "", new{ autocomplete="off" })%>
    </p>
    <p>
        <label for="card.expirymonth">Expire Date</label>
        <%= Html.DropDownList("card.expirymonth", Card.Months.ToStringValues().AsSelectList(ViewData.Model.Order.Card.ExpiryMonth))%>
        <%= Html.DropDownList("card.expiryyear", Card.ExpiryYears.ToStringValues().AsSelectList(ViewData.Model.Order.Card.ExpiryYear))%>
    </p>
    <p>
        <label for="card.startmonth">Start Date (if present)</label>
        <%= Html.DropDownList("card.startmonth", Card.Months.ToStringValues().AddBlankFirstValue().AsSelectList(ViewData.Model.Order.Card.StartMonth))%>
        <%= Html.DropDownList("card.startyear", Card.StartYears.ToStringValues().AddBlankFirstValue().AsSelectList(ViewData.Model.Order.Card.StartYear))%>
    </p>
    <p>
        <label for="card.issuenumber">Issue Number (if present)</label>
        <%= Html.TextBox("card.issuenumber", ViewData.Model.Order.Card.IssueNumber, new { maxlength = "1", @class="small" })%>
    </p>
    <p>
        <label for="card.securitycode">Security Code</label>
        <%= Html.TextBox("card.securitycode", "", new { maxlength = "4", @class="small" })%>
    </p>
    <p>
        The last three digits printed on the signature strip of your credit/debit card. Or for Amex. the four digits printed on the front of the card above the embossed card number.
    </p>
</div>
<p id="dontPayOnline">
	<label for="order.paybytelephone"><strong>I prefer to pay by cheque or telephone</strong></label>
	<%= Html.CheckBox("order.paybytelephone", ViewData.Model.Order.PayByTelephone,
				new { onclick = "javascript:toggleCard();" })%>
</p>
<p>If you tick this option you will receive an order number. You should quote this number when you contact us with your payment. We accept most major credit and debit cards including Amex. We also accept cheques( in pounds sterling from a UK bank only) and postal orders made payable to ‘Jump the Gun’ Please note that cheques will have to await clearance before we can dispatch goods- this can take six working days.</p>
<div id="terms">
	<p>All credit card details are collected using an SSL secure server which means that any sensitive information is securely encrypted and cannot be read by any unauthorised party. We do not store any of your credit card details once payment has been taken. We will not take payment until your goods are ready for dispatch.</p>
</div>
<p>Would you like to be emailed with our newsletter and product information? (Your contact information will not be shared with 3rd parties)</p>
<p>
<select name="order.contactme">
	<option value="false">No</option>
	<option value="true">Yes</option>
</select>
</p>