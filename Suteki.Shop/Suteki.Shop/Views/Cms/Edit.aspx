<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/CmsSubMenu.master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.ViewPage<CmsViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div class="admin-form">
<h1>Content</h1>

<p><%= Model.Content.Link(Html)%></p>

<%= Html.MessageBox(Model)%>
<%= Html.ValidationSummary() %>

<% using (Html.BeginForm()) { %>
	<%= this.Hidden(x => x.Content.ContentTypeId) %>
	<%= this.Hidden(x => x.Content.ContentId) %>
	<%= this.Hidden(x => x.Content.Position) %>
	<p><%= this.TextBox(x => x.Content.Name).Label("Name") %></p>
	<p><%= this.TextArea(x => (x.Content as ITextContent).Text).Label("Text") %></p>
	<p><%= this.Select(x => x.Content.ParentContentId).Options(Model.Menus, x => x.ContentId, x => x.Name).Label("Parent Menu") %></p>
	<p><%= this.CheckBox(x => x.Content.IsActive).Label("Active") %></p>
	
	<input type="submit" value="Save Changes" />
<% } %>

<% Html.InitialiseRichTextEditor(); %>
</div>
</asp:Content>
