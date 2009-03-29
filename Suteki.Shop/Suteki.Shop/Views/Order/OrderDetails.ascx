<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ShopViewData>" %>
 <h3>Order Details</h3>

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
        <%= Html.ActionLink<OrderStatusController>(c => c.Dispatch(ViewData.Model.Order.OrderId), "Dispatch", new { _class = "linkButton" })%>
        <%= Html.ActionLink<OrderStatusController>(c => c.Reject(ViewData.Model.Order.OrderId), "Reject", new { _class = "linkButton" })%>
    <% } else { %>
        <%= Html.ActionLink<OrderStatusController>(c => c.UndoStatus(ViewData.Model.Order.OrderId), "Reset Status", new { _class = "linkButton" })%>
    <% } %>
    <%= Html.ActionLink<OrderController>(c => c.Invoice(ViewData.Model.Order.OrderId), "Print Invoice", new { _class = "linkButton" }) %>
<% } %>
</div>    

  
<table>
    <tr>
        <th class="wide">Product</th>
        <th class="thin">Size</th>
        <th class="thin number">Quantity</th>
        <th class="thin number">Unit Price</th>
        <th class="thin number">Total Price</th>
    </tr>
    
    <% foreach (var basketItem in ViewData.Model.Order.Basket.BasketItems)
       { %>
    
    <tr>
        <td><%= Html.ActionLink<ProductController>(c => c.Item(basketItem.Size.Product.UrlName), basketItem.Size.Product.Name)%></td>
        <td><%= basketItem.Size.Name%></td>
        <td class="number"><%= basketItem.Quantity%></td>
        <td class="number"><%= basketItem.Size.Product.Price.ToString("£0.00")%></td>
        <td class="number"><%= basketItem.Total.ToString("£0.00")%></td>
    </tr>
    
    <% } %>
    
    <tr class="total">
        <td>Total</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td class="number"><%= ViewData.Model.Order.Basket.Total.ToString("£0.00")%></td>
    </tr>

    <tr>
        <td>Postage</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td class="number"><%= ViewData.Model.Order.Basket.PostageTotal%></td>
        <td>&nbsp;</td>
    </tr>

    <tr>
        <td>(for <%= ViewData.Model.Order.Basket.Country.Name %>)</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
    </tr>

    <tr class="total">
        <td>Total With Postage</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td>&nbsp;</td>
        <td class="number"><%= ViewData.Model.Order.Basket.TotalWithPostage%></td>
        <td>&nbsp;</td>
    </tr>
    
</table>