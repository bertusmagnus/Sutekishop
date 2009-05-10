<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<%@ Import Namespace="MvcContrib"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div class="admin-form">
	<h1>Orders</h1>
	<% Html.RenderPartial("OrderSearchForm"); %>
	<div class="pager">
		<%= Html.Pager("Order", "Index", Model.Orders)%>
	</div>

	<%= Html.Grid(Model.Orders).Columns(column => {
			column.For(x => Html.ActionLink<OrderController>(c => c.Item(x.OrderId), x.OrderId.ToString()))
				.DoNotEncode().Named("Number").HeaderAttributes(@class => "thin");
			column.For(x => x.Contact.Fullname).Named("Customer").HeaderAttributes(@class => "wide");
			column.For(x => x.CreatedDate.ToShortDateString()).Named("Created");
			column.For(x => x.DispatchedDateAsString).Named("Dispatched").HeaderAttributes(@class => "thin").DoNotEncode();
			column.For(x => x.OrderStatus.Name).Named("Status").HeaderAttributes(@class => "thin");
			column.For(x => x.UserAsString).Named("Updated by").HeaderAttributes(@class => "thin").DoNotEncode();
	}).RowAttributes(row => new Hash(@class => row.Item.OrderStatus.Name)) %>

	<p>Total orders: <%= Model.Orders.TotalCount %></p>
</div>
</asp:Content>
