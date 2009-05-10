<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ShopViewData>" %>
<%@ Import Namespace="Suteki.Common.Models"%>

        <%= Html.Hidden("order.orderid", ViewData.Model.Order.OrderId.ToString())%>
        <%= Html.Hidden("order.basketid", ViewData.Model.Order.BasketId.ToString())%>
        
        <!-- card contact -->

        
        <h3>Card Holder</h3>
        
            <p>
                <label for="cardcontact.firstname">First Name</label>
                <%= Html.TextBox("cardcontact.firstname", ViewData.Model.Order.Contact.Firstname)%>
            </p>
            <p>
                <label for="cardcontact.lastname">Last Name</label>
                <%= Html.TextBox("cardcontact.lastname", ViewData.Model.Order.Contact.Lastname)%>
            </p>
            <p>
                <label for="cardcontact.address1">Address</label>
                <%= Html.TextBox("cardcontact.address1", ViewData.Model.Order.Contact.Address1)%>
            </p>
            <p>
                <label for="cardcontact.address2">&nbsp;</label>
                <%= Html.TextBox("cardcontact.address2", ViewData.Model.Order.Contact.Address2)%>
            </p>
            <p>
                <label for="cardcontact.address3">&nbsp;</label>
                <%= Html.TextBox("cardcontact.address3", ViewData.Model.Order.Contact.Address3)%>
            </p>
            <p>
                <label for="cardcontact.town">Town / City</label>
                <%= Html.TextBox("cardcontact.town", ViewData.Model.Order.Contact.Town)%>
            </p>
            <p>
                <label for="cardcontact.county">County</label>
                <%= Html.TextBox("cardcontact.county", ViewData.Model.Order.Contact.County)%>
            </p>
            <p>
                <label for="cardcontact.postcode">Postcode</label>
                <%= Html.TextBox("cardcontact.postcode", ViewData.Model.Order.Contact.Postcode)%>
            </p>
            <p>
                <label for="cardcontact.countryid">Country*</label>
                <%= Html.DropDownList("cardcontact.countryid", new SelectList(ViewData.Model.Countries, "CountryId", "Name", ViewData.Model.Order.CardContactCountryId))%>
            </p>
            <p>
                <label for="cardcontact.telephone">Telephone</label>
                <%= Html.TextBox("cardcontact.telephone", ViewData.Model.Order.Contact.Telephone)%>
            </p>
            <p>
                <label for="order.email">Email</label>
                <%= Html.TextBox("order.email", ViewData.Model.Order.Email)%>
            </p>
            <p>
                <label for="emailconfirm">Confirm Email</label>
                <%= Html.TextBox("emailconfirm")%>
            </p>
 
        <!-- deliver contact -->
        
        <h3>Delivery Address</h3>
        
        <p>
            <label for="order.usecardholdercontact">Use Cardholder Details</label>
            <%= Html.CheckBox("order.usecardholdercontact", ViewData.Model.Order.UseCardHolderContact, new { onclick = "javascript:toggleCardHolderDetails();" })%>
        </p>
        <div id="deliveryAddress">
            <p>
                <label for="deliverycontact.firstname">First Name</label>
                <%= Html.TextBox("deliverycontact.firstname", ViewData.Model.Order.Contact1.Firstname)%>
            </p>
            <p>
                <label for="deliverycontact.lastname">Last Name</label>
                <%= Html.TextBox("deliverycontact.lastname", ViewData.Model.Order.Contact1.Lastname)%>
            </p>
            <p>
                <label for="deliverycontact.address1">Address</label>
                <%= Html.TextBox("deliverycontact.address1", ViewData.Model.Order.Contact1.Address1)%>
            </p>
            <p>
                <label for="deliverycontact.address2">&nbsp;</label>
                <%= Html.TextBox("deliverycontact.address2", ViewData.Model.Order.Contact1.Address2)%>
            </p>
            <p>
                <label for="deliverycontact.address3">&nbsp;</label>
                <%= Html.TextBox("deliverycontact.address3", ViewData.Model.Order.Contact1.Address3)%>
            </p>
            <p>
                <label for="deliverycontact.town">Town / City</label>
                <%= Html.TextBox("deliverycontact.town", ViewData.Model.Order.Contact1.Town)%>
            </p>
            <p>
                <label for="deliverycontact.county">County</label>
                <%= Html.TextBox("deliverycontact.county", ViewData.Model.Order.Contact1.County)%>
            </p>
            <p>
                <label for="deliverycontact.postcode">Postcode</label>
                <%= Html.TextBox("deliverycontact.postcode", ViewData.Model.Order.Contact1.Postcode)%>
            </p>
            <p>
                <label for="deliverycontact.countryid">Country</label>
                <%= Html.DropDownList("deliverycontact.countryid", new SelectList(ViewData.Model.Countries, "CountryId", "Name", ViewData.Model.Order.DeliveryContactCountryId))%>
            </p>
            <p>
                <label for="deliverycontact.telephone">Telephone</label>
                <%= Html.TextBox("deliverycontact.telephone", ViewData.Model.Order.Contact1.Telephone)%>
            </p>
        
        </div>
        
        <!-- additional information -->  
        
        <p>
            <label for="order.additionalinformation">Additional Information</label>
            <span><%= Html.TextArea("order.additionalinformation", ViewData.Model.Order.AdditionalInformation)%></span>
        </p>
        
* Selecting a different country may result in a different postage charge. You will be able to review this on the next page.