<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <h2>Join our Mailing List</h2>
    
    <%= Html.ValidationSummary() %>
    
	<p>
		Please fill out the form below if you would like to sign up to our mailing list. 
		Your information will not be shared with any third parties.
	</p>
	
	<% using(Html.BeginForm()) { %>
	 <table>
            <tr>
                <td class="label"><label for="<%= this.IdFor(x => x.MailingListSubscription.Contact.Firstname) %>">First Name</label></td>
                <td class="field"><%= this.TextBox(x => x.MailingListSubscription.Contact.Firstname) %> </td>
            </tr>
            <tr>
                <td class="label"><label for="<%= this.IdFor(x => x.MailingListSubscription.Contact.Lastname) %>">Last Name</label></td>
                <td class="field"><%= this.TextBox(x => x.MailingListSubscription.Contact.Lastname) %></td>
            </tr>
            <tr>
                <td class="label"><label for="<%= this.IdFor(x => x.MailingListSubscription.Contact.Address1) %>">Address</label></td>
                <td class="field"><%= this.TextBox(x => x.MailingListSubscription.Contact.Address1) %></td>
            </tr>
            <tr>
                <td class="label">&nbsp;</td>
                <td class="field"><%= this.TextBox(x => x.MailingListSubscription.Contact.Address2) %></td>
            </tr>
            <tr>
                <td class="label">&nbsp;</td>
                <td class="field"><%= this.TextBox(x => x.MailingListSubscription.Contact.Address3) %></td>
            </tr>
            <tr>
                <td class="label"><label for="<%= this.IdFor(x => x.MailingListSubscription.Contact.Town) %>">Town / City</label></td>
                <td class="field"><%= this.TextBox(x => x.MailingListSubscription.Contact.Town) %></td>
            </tr>
            <tr>
                <td class="label"><label for="<%= this.IdFor(x => x.MailingListSubscription.Contact.County) %>">County</label></td>
                <td class="field"><%= this.TextBox(x => x.MailingListSubscription.Contact.County) %></td>
            </tr>
            <tr>
                <td class="label"><label for="<%= this.IdFor(x => x.MailingListSubscription.Contact.Postcode) %>">Postcode</label></td>
                <td class="field"><%= this.TextBox(x => x.MailingListSubscription.Contact.Postcode) %></td>
            </tr>
            <tr>
                <td class="label"><label for="<%= this.IdFor(x => x.MailingListSubscription.Contact.CountryId) %>">Country</label></td>
                <td class="field"><%= this.Select(x => x.MailingListSubscription.Contact.CountryId).Options(Model.Countries, x => x.CountryId, x => x.Name) %></td>
            </tr>
            <tr>
                <td class="label"><label for="<%= this.IdFor(x => x.MailingListSubscription.Contact.Telephone) %>">Telephone</label></td>
                <td class="field"><%= this.TextBox(x => x.MailingListSubscription.Contact.Telephone) %></td>
            </tr>
            <tr>
                <td class="label"><label for=<%= this.IdFor(x => x.MailingListSubscription.Email) %>>Email</label></td>
                <td class="field"><%= this.TextBox(x => x.MailingListSubscription.Email) %></td>
            </tr>
            <tr>
                <td class="label"><label for="emailconfirm">Confirm Email</label></td>
                <td class="field"><%= Html.TextBox("emailconfirm") %></td>
            </tr>
        </table>
        
        <input type="submit" value="Sign Up" />
	<% } %>	
	<script type="text/javascript">
		$(function() {
			$('#<%= this.IdFor(x => x.MailingListSubscription.Contact.Firstname) %>').focus();
		});
	</script>
</asp:Content>
