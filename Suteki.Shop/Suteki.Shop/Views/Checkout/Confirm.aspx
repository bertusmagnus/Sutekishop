<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<h1>Final Confirmation</h1>
	<p>When you are happy that your Order Details are correct, please press the 'Place Order' button.</p>
	
	
	<% Html.RenderPartial("~/Views/Order/OrderDetails.ascx"); %>

	<% Html.RenderPartial("~/Views/Order/CustomerDetails.ascx"); %>
	
	<div class="clear"></div>
		
	<% using (Html.BeginForm()) { %>
		<%= this.Hidden(x => x.Order.OrderId) %>
		<input type="submit" value="Place Order" />
	<% } %>
</asp:Content>
