<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h2>Select Postage Country</h2>
    
    <% using(Html.BeginForm()) { %>
		<%= this.Select(x=>x.Basket.CountryId).Options(Model.Countries, x => x.CountryId, x =>x.Name) %>
		<%= this.Hidden(x=>x.Basket.BasketId) %>
		<input type="submit" value="Change Country" />
    <% } %>

</asp:Content>
