<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <h2>Edit Mailing List Subscription</h2>

	<%= Html.ValidationSummary() %>

	<% using(Html.BeginForm()) { %>
		<%= this.Hidden(x => x.MailingListSubscription.Id) %>
		
		<%= this.TextBox(x => x.MailingListSubscription.Contact.Firstname).Label("First Name") %>
		<%= this.TextBox(x => x.MailingListSubscription.Contact.Lastname).Label("Last Name") %>
        <%= this.TextBox(x => x.MailingListSubscription.Contact.Address1).Label("Address") %>
        <%= this.TextBox(x => x.MailingListSubscription.Contact.Address2) %>
        <%= this.TextBox(x => x.MailingListSubscription.Contact.Address3) %>
        <%= this.TextBox(x => x.MailingListSubscription.Contact.Town).Label("Town / City") %>
        <%= this.TextBox(x => x.MailingListSubscription.Contact.County).Label("County") %>
        <%= this.TextBox(x => x.MailingListSubscription.Contact.Postcode).Label("Postcode") %>
        <%= this.Select(x => x.MailingListSubscription.Contact.CountryId).Options(Model.Countries, x => x.CountryId, x => x.Name).Label("Country") %>
        <%= this.TextBox(x => x.MailingListSubscription.Contact.Telephone).Label("Telephone") %>
		
		<%= this.TextBox(x => x.MailingListSubscription.Email).Label("Email") %>
		
		<input type="submit" value="Save Changes" />
	<% } %>
	
	
</asp:Content>
