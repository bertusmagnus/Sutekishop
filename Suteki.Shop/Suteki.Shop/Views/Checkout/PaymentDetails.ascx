<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ShopViewData>" %>
<%@ Import Namespace="Suteki.Common.Models"%>

        
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
                    <td colspan="2" class="field small"><%= Html.TextBox("card.securitycode", "", new { maxlength = "4" })%></td>
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
