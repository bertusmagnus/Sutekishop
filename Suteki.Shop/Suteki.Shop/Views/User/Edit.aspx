<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.master" AutoEventWireup="true" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div class="admin-form">
    <h1>User</h1>
    
	<%= Html.ValidationSummary() %>
    <%= Html.MessageBox(Model) %>

    <% using(Html.BeginForm()) { %>
		<p><%= this.TextBox(x => x.User.Email).Label("Email") %></p>
		<%-- NOTE: We lose modelstate support for password as we're overriding the name --%>
		<p><%= this.Password(x=> x.User.Password).Name("password").Value("").Label("Password") %></p>
		<p><%= this.Select(x => x.User.RoleId).Options(Model.Roles, x => x.RoleId, x => x.Name).Label("Role") %></p>
        <p><%= this.CheckBox(x => x.User.IsEnabled).Label("User can log on") %></p>
		<p>
			<%= this.Hidden(x => x.User.UserId) %>
			<input type="submit" value="Save" />
		</p>

    <% } %>
</div>
</asp:Content>
