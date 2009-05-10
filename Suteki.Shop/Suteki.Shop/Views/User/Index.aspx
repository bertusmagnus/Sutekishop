<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div class="admin-form">
    <p><%= Html.ActionLink<UserController>(c => c.New(), "New User") %></p>

	<%= Html.Grid(Model.Users).Columns(column => {
			column.For(x => Html.ActionLink<UserController>(c => c.Edit(x.UserId), x.Email)).Named("Email").DoNotEncode();
			column.For(x => x.Role.Name).Named("Role");
			column.For(x => Html.Tick(x.IsEnabled)).DoNotEncode();
   	}) %>
</div>
</asp:Content>
