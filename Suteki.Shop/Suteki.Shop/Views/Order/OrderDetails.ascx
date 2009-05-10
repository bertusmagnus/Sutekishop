<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ShopViewData>" %>
  
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
    </tr>

    <tr>
        <td>(for <%= ViewData.Model.Order.Basket.Country.Name %>)</td>
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
    </tr>
    
</table>