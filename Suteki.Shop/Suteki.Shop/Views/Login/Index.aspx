<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <h1>Login</h1>
    <% using(Html.BeginForm()) { %>
        <p><label for="email">Email</label><%= Html.TextBox("email") %></p>
        <p><label for="password">Password</label><%= Html.Password("password") %></p>
		<p><%= Html.SubmitButton("btnLogin", "login")%></p>
	<% } %>
	<p><strong><%= Html.ErrorBox(Model)%></strong></p>
	<script type="text/javascript">
		$(function() {
			$('#email').focus();
		});
	</script>
</asp:Content>
