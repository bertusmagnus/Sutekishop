<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ShopViewData>" %>
    <p><%= Html.ActionLink<OrderController>(c => c.Print(ViewData.Model.Order.OrderId), "Printable version of this page") %></p>
<dl>
    <dt>Order Number</dt><dd><%= ViewData.Model.Order.OrderId.ToString()%>&nbsp;</dd>
    <dt>Date</dt><dd><%= ViewData.Model.Order.CreatedDate.ToShortDateString()%></dd>

<% if(ViewContext.HttpContext.User.IsAdministrator()) { %>
    <dt>Status</dt><dd><%= ViewData.Model.Order.OrderStatus.Name %> <%= ViewData.Model.Order.DispatchedDateAsString %></dd>
    <dt>Updated by</dt><dd><%= ViewData.Model.Order.UserAsString %></dd>
<% } %>
    
</dl>

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

