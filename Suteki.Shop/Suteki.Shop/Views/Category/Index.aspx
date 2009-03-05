<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
	<p><%= Html.ActionLink<CategoryController>(c => c.New(1), "New Category")%></p>
	
	<%= Html.WriteCategories(ViewData.Model.Category, CategoryDisplay.Edit)%>
</asp:Content>
