<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ShopViewData>" %>
<%@ Import Namespace="Suteki.Common.Models"%>

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
                <td class="label"><label for="cardcontact.countryid">Country*</label></td>
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
                <td class="field"><%= Html.TextBox("emailconfirm")%></td>
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
* Selecting a different country may result in a different postage charge. You will be able to review this on the next page.