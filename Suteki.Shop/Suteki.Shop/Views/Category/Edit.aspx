<%@ Page Title="Edit Category" Language="C#" MasterPageFile="~/Views/Shared/Shop.master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div class="admin-form">
    <h1>Category</h1>
    
    <%= Html.ValidationSummary() %>
    <%= Html.MessageBox(Model)%>

	<% using(Html.BeginForm()) { %>
		<p><%= this.TextBox(x => x.Category.Name).Label("Name") %></p>
		<p><%= this.Select(x => x.Category.ParentId).Options(Model.Categories, x => x.CategoryId, x => x.Name).Label("Parent Category") %></p>
        <p><%= this.CheckBox(x => x.Category.IsActive).Label("Active") %></p>
        <p>
			<%= this.Hidden(x => x.Category.CategoryId) %>
			<%= this.Hidden(x => x.Category.Position) %>
			<%= Html.SubmitButton() %>
        </p>
    <% } %>
</div>
</asp:Content>