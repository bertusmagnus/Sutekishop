<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ShopViewData>" %>
<h3>Customer Details</h3>
<div id="customer-details">
	<h3>Card Holder</h3>
    <p><span>First Name</span><%= Model.Order.Contact.Firstname %></p>
    <p><span>Last Name</span><%= Model.Order.Contact.Lastname %></p>
    <p><span>Address 1</span><%= Model.Order.Contact.Address1 %></p>
    <p><span>Address 2</span><%= Model.Order.Contact.Address2 %></p>
    <p><span>Address 3</span><%= Model.Order.Contact.Address3 %></p>
    <p><span>Town</span><%= Model.Order.Contact.Town %></p>
    <p><span>County</span><%= Model.Order.Contact.County %></p>
    <p><span>Postcode</span><%= Model.Order.Contact.Postcode %></p>
    <p><span>Country</span><%= Model.Order.Contact.Country.Name %></p>
    <p><span>Telephone</span><%= Model.Order.Contact.Telephone %></p>
    <p><span>Email</span><%= Html.Mailto(Model.Order.Email, ViewData.Model.Order.Email) %></p>
    <p><span>Newsletter</span><%= Model.Order.ContactMe.ToYesNo() %></p>
        
    <!-- deliver contact -->
    <h3>Delivery Address</h3>

    <% if(Model.Order.UseCardHolderContact) { %>
        <p>Use Card Holder Contact</p>
    <% } else { %>
        <p><span>First Name</span><%= Model.Order.Contact1.Firstname %></p>
        <p><span>Last Name</span><%= Model.Order.Contact1.Lastname %></p>
        <p><span>Address 1</span><%= Model.Order.Contact1.Address1 %></p>
        <p><span>Address 2</span><%= Model.Order.Contact1.Address2 %></p>
        <p><span>Address 3</span><%= Model.Order.Contact1.Address3 %></p>
        <p><span>Town</span><%= Model.Order.Contact1.Town %></p>
        <p><span>County</span><%= Model.Order.Contact1.County %></p>
        <p><span>Postcode</span><%= Model.Order.Contact1.Postcode %></p>
        <p><span>Country</span><%= Model.Order.Contact1.Country.Name %></p>
        <p><span>Telephone</span><%= Model.Order.Contact1.Telephone %></p>
    <% } %>
        
    <p>
		<span>Additional Information</span>
		<%= string.IsNullOrEmpty(Model.Order.AdditionalInformation) ? "None Supplied" : Model.Order.AdditionalInformation %>
	</p>
</div>