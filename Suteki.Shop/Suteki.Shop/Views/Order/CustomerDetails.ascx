<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ShopViewData>" %>
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
            <dt>Newsletter</dt><dd><%= Model.Order.ContactMe.ToYesNo() %></dd>
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
        
        
    </div>      
</div>
<div class="clear">
    <h3>Additional Information</h3>
    <p><%= ViewData.Model.Order.AdditionalInformation %></p>
</div>
