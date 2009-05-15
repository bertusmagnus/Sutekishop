<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>Stock</h1>

<div class="columnContainer">

<% using(Html.BeginForm()) { %>

    <%= Html.WriteStock(Model.Category) %>

    <%= Html.SubmitButton() %>

<% } %>

</div>

</asp:Content>
