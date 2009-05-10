<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ShopViewData>" %>
    <p><%= Html.ActionLink<OrderController>(c => c.Print(ViewData.Model.Order.OrderId), "Printable version of this page") %></p>

<div id="order-details">
    <p><span>Order Number</span><%= ViewData.Model.Order.OrderId.ToString()%></p>
    <p><span>Date</span><%= ViewData.Model.Order.CreatedDate.ToShortDateString()%></p>

<% if(ViewContext.HttpContext.User.IsAdministrator()) { %>
    <p><span>Status</span><%= ViewData.Model.Order.OrderStatus.Name %> <%= ViewData.Model.Order.DispatchedDateAsString %></p>
    <p><span>Updated by</span><%= ViewData.Model.Order.UserAsString %></p>
<% } %>
</div>

<div class="orderAction">
<% if(ViewContext.HttpContext.User.IsAdministrator()) { %>
    <% if(ViewData.Model.Order.IsCreated) { %>
        <%= Html.ActionLink<OrderStatusController>(c => c.Dispatch(ViewData.Model.Order.OrderId), "Dispatch (and send dispatch notification)", new { @class = "linkButton" })%>
        <%= Html.ActionLink<OrderStatusController>(c => c.Reject(ViewData.Model.Order.OrderId), "Reject", new { @class = "linkButton" })%>
    <% } else { %>
        <%= Html.ActionLink<OrderStatusController>(c => c.UndoStatus(ViewData.Model.Order.OrderId), "Reset Status", new { @class = "linkButton" })%>
    <% } %>
    <%= Html.ActionLink<InvoiceController>(c => c.Show(ViewData.Model.Order.OrderId), "Print Invoice", new { @class = "linkButton" }) %>
<% } %>
</div>    

