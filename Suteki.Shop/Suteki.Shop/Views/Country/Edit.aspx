<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.master" Inherits="Suteki.Shop.ViewPage<ScaffoldViewData<Suteki.Shop.Country>>" %>
<%@ Import Namespace="Suteki.Common.ViewData"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div class="admin-form">
    <h1>Country</h1>
    <%= Html.ValidationSummary() %>
    <%= Html.MessageBox(Model)%>

    <% using (Html.BeginForm()) { %>
		<p><%= this.TextBox(x => x.Item.Name).Label("Name") %></p>
		<p><%= this.CheckBox(x => x.Item.IsActive).Label("Active") %></p>
		<p><%= this.Select(x => x.Item.PostZoneId).Options(Model.GetLookupList<PostZone>(), x => x.PostZoneId, x => x.Name).Label("Post Zone")%></p>
		<p>
			<%= this.Hidden(x => x.Item.CountryId) %>
			<%= this.Hidden(x => x.Item.Position) %>
			<input type="submit" value="Save" />
		</p>
    <% } %>
</div>
</asp:Content>
