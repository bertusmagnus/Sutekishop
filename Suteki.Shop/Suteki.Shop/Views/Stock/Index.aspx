<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div class="admin-form">
<h1>Stock</h1>
	<% using(Html.BeginForm()) { %>
		<%= Html.WriteStock(Model.Category) %>
		<p><%= Html.SubmitButton() %></p>
	<% } %>
</div>
</asp:Content>
