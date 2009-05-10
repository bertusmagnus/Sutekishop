<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ShopViewData>" %>
<div id="payment-details">
	<h3>Payment Details</h3>
	<% if(Model.Order.PayByTelephone) { %>
	   
			<p>Pay By Telephone</p>
	        
			<% } else { %>

		<p><span>Card Type</span><%= Model.Order.Card.CardType.Name %></p>
		<p><span>Card Holder</span><%= Model.Order.Card.Holder %></p>

		<% if(ViewContext.HttpContext.User.IsAdministrator()) { %>

			<%= Html.ErrorBox(Model) %>

			<% if (Model.Card == null) { %>

				<% using (Html.BeginForm("ShowCard", "Order", FormMethod.Post,
					   new Dictionary<string, object> { { "onsubmit", "submitHandler();" } }))
				   { %>
	                
					<%= Html.Hidden("orderId", Model.Order.OrderId.ToString()) %>
	                
					<p>
					<label for="privateKey">Private Key</label>
					<%= Html.TextBox("privateKey")%>
	                </p>
	                
					<p><%= Html.SubmitButton("cardDetailsSubmit", "Get Card Details")%></p>

				<% } %>
	        
			<% } else { %>
					<p><span>Card Number</span><%= Model.Card.Number %></p>
					<p><span>Issue Number</span><%= Model.Card.IssueNumber %></p>
					<p><span>Security Code</span><%= Model.Card.SecurityCode %></p>
					<p><span>Start Date</span><%= Model.Card.StartDateAsString %></p>
					<p><span>Expiry Date</span><%= Model.Card.ExpiryDateAsString %></p>
			<% } %>
		<% } %>        
	<% } %>      
</div>
