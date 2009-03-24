<%@ Control Language="C#" Inherits="Suteki.Shop.ViewUserControl<ShopViewData>" %>
<% using(Html.BeginForm()) { %>

    <div class="contentLeftColumn">
		<%= this.TextBox(x => x.OrderSearchCriteria.OrderId).Label("Order Number") %>
		<%= this.TextBox(x => x.OrderSearchCriteria.Email).Label("Email") %>
		<%= this.TextBox(x => x.OrderSearchCriteria.Lastname).Label("Last Name") %>

		<input type="submit" value="Search" />
    </div>

    <div class="contentRightColumn">
		<%= this.TextBox(x => x.OrderSearchCriteria.Postcode).Label("Postcode") %>
    
        <label for="orderstatusid">Status</label>
        <select id="orderstatusid" name="orderstatusid">
            <option value="0">Any</option>
            <option value="1">Created</option>
            <option value="2">Dispatched</option>
            <option value="3">Rejected</option>
        </select>
        
        <br /><br /><br /><br />
    </div>

<% } %>