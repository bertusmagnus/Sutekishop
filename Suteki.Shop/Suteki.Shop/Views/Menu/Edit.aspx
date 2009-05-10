<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/CmsSubMenu.master" Inherits="Suteki.Shop.ViewPage<CmsViewData>" %>

<asp:Content ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<h1>Menu</h1>

<%= Html.ValidationSummary() %>

<% if (Model.Content.ContentId > 0) { %>
	<p><%= Model.Content.Link(Html)%></p>
<% } %>

<% using(Html.BeginForm()) { %>
	<%= this.Hidden(x => x.Content.ContentId) %>
	<%= this.Hidden(x => x.Content.ContentTypeId) %>
	<%= this.TextBox(x => x.Content.Name).Label("Name") %>
	<%= this.Select(x => x.Content.ParentContentId).Options(Model.Menus, x => x.ContentId, x => x.Name).Label("Parent Menu") %>
	<%= this.CheckBox(x => x.Content.IsActive).Label("Active") %>
	
	<input type="submit" value="Save Changes" />
<% } %>
</asp:Content>
