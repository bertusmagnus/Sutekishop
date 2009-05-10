<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.master"   Inherits="Suteki.Shop.ViewPage<ScaffoldViewData<Suteki.Shop.Postage>>" %>
<%@ Import Namespace="Suteki.Common.ViewData"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div class="admin-form">
    <h1>Postage Band</h1>
    <%= Html.ValidationSummary() %>
    <%= Html.MessageBox(Model) %>

    <% using (Html.BeginForm()) { %>
		<p><%= this.TextBox(x => x.Item.Name).Label("Name") %></p>
		<p><%= this.TextBox(x => x.Item.MaxWeight).Label("Max Weight (grams)") %></p>
        <p><%= this.TextBox(x => x.Item.Price).Format("0.00").Label("Price") %></p>
        <p><%= this.CheckBox(x=>x.Item.IsActive).Label("Active") %></p>
        <p>
			<%= this.Hidden(x=>x.Item.PostageId) %>
			<%= this.Hidden(x => x.Item.Position) %>
			<input type="submit" value="Save Changes" />
        </p>
    <% } %>
</div>
</asp:Content>
