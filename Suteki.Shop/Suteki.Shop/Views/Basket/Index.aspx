<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>Basket</h1>

<%= Html.ValidationSummary() %>

<% if(Model.Basket.IsEmpty) { %>

    <p>Your basket is empty</p>

<% } else { %>

    <table>
        <tr>
            <th class="wide">Product</th>
            <th class="thin">Size</th>
            <th class="thin number">Quantity</th>
            <th class="thin number">Unit Price</th>
            <th class="thin number">Total Price</th>
            <th class="thin number">Delete</th>
        </tr>
        
        <% foreach (var basketItem in Model.Basket.BasketItems) { %>
        
        <tr>
            <td><%= Html.ActionLink<ProductController>(c => c.Item(basketItem.Size.Product.UrlName), basketItem.Size.Product.Name)%></td>
            <td><%= basketItem.Size.Name%></td>
            <td class="number"><%= basketItem.Quantity%></td>
            <td class="number"><%= basketItem.Size.Product.Price.ToString("£0.00")%></td>
            <td class="number"><%= basketItem.Total.ToString("£0.00")%></td>
            <td class="number"><%= Html.ActionLink<BasketController>(c => c.Remove(basketItem.BasketItemId), "X")%></td>
        </tr>
        
        <% } %>
        
        <tr class="total">
            <td>Total</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td class="number"><%= Model.Basket.Total.ToString("£0.00")%></td>
            <td>&nbsp;</td>
        </tr>

        <tr>
            <td>Postage</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td>&nbsp;</td>
            <td class="number"><%= Model.Basket.PostageTotal%></td>
            <td>&nbsp;</td>
        </tr>

        <tr>
            <td>(for <%= Model.Basket.Country.Name %> - <%= Html.ActionLink<BasketController>(c=>c.ChangeCountry(), "change") %>)</td>
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
            <td class="number"><%= Model.Basket.TotalWithPostage%></td>
            <td>&nbsp;</td>
        </tr>
        
    </table>

    <p>The default postage & package charge displayed is for UK postal deliveries. If you select a delivery address outside the UK please check this price again.</p>

    <%= Html.ActionLink<CheckoutController>(c => c.Index(Model.Basket.BasketId), "Checkout", new { @class = "linkButton" }).ToSslLink() %>

<% } %>

</asp:Content>
