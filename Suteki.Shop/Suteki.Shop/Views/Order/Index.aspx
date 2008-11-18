<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.Order.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>Orders</h1>

<div class="columnContainer">
<% using(Html.BeginForm<OrderController>(c => c.Index())) { %>

    <div class="contentLeftColumn">
        <label for="orderid">Order Number</label>
        <%= Html.TextBox("orderid", ViewData.Model.OrderSearchCriteria.OrderId) %>

        <label for="email">Email</label>
        <%= Html.TextBox("email", ViewData.Model.OrderSearchCriteria.Email)%>

        <label for="lastname">Last Name</label>
        <%= Html.TextBox("lastname", ViewData.Model.OrderSearchCriteria.Lastname)%>

        <%= Html.SubmitButton("searchSubmit", "Search") %>
    </div>

    <div class="contentRightColumn">
        <label for="postcode">Postcode</label>
        <%= Html.TextBox("postcode", ViewData.Model.OrderSearchCriteria.Postcode)%>
    
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

</div>

<div class="clearRight">&nbsp;</div>

<div class="columnContainer">
    <div class="pager">
    <%= Html.Pager("Order", "Index", ViewData.Model.Orders, ViewData.Model.OrderSearchCriteria)%>
    </div>

    <table>

        <tr>
            <th class="thin">Number</th>
            <th class="wide">Customer</th>
            <th class="thin">Created</th>
            <th class="thin">Dispatched</th>
            <th class="thin">Status</th>
            <th class="thin">Updated by</th>
        </tr>

    <% foreach(var order in ViewData.Model.Orders) { %>

        <tr class="<%= order.OrderStatus.Name %>">
            <td><%= Html.ActionLink<OrderController>(c => c.Item(order.OrderId), order.OrderId.ToString()) %></td>
            <td><%= order.Contact.Fullname %></td>
            <td><%= order.CreatedDate.ToShortDateString() %></td>
            <td><%= order.DispatchedDateAsString %></td>
            <td><%= order.OrderStatus.Name %></td>
            <td><%= order.UserAsString %></td>
        </tr>

    <% } %>
    </table>

    <p>Total orders: <%= ViewData.Model.Orders.TotalCount %></p>
</div>
</asp:Content>
