<%@ Control Language="C#" Inherits="Suteki.Shop.ViewUserControl<ShopViewData>" %>
<% using(Html.BeginForm()) { %>

    <div>
		<p><%= this.TextBox(x => x.OrderSearchCriteria.OrderId).Label("Order Number") %></p>
		<p><%= this.TextBox(x => x.OrderSearchCriteria.Email).Label("Email") %></p>
		<p><%= this.TextBox(x => x.OrderSearchCriteria.Lastname).Label("Last Name") %></p>
		<p><%= this.TextBox(x => x.OrderSearchCriteria.Postcode).Label("Postcode") %></p>  
        <p><%= this.Select(x => x.OrderSearchCriteria.OrderStatusId).Label("Status").Options(Model.OrderStatuses, x => x.OrderStatusId, x => x.Name) %></p>
        <p><input type="submit" value="Search" /></p>
    </div>

<% } %>