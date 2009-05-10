<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div class="admin-form">
    <h1>Category List</h1>	
	<%= Html.MessageBox(Model)%>
	
	<p><%= Html.ActionLink<CategoryController>(c => c.New(1), "Add a new category")%></p>	
	<%= Html.WriteCategories(Model.Category, CategoryDisplay.Edit)%>
	
</div>
</asp:Content>
