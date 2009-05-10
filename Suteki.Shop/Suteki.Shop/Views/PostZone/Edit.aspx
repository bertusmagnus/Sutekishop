<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.master" Inherits="Suteki.Shop.ViewPage<ScaffoldViewData<Suteki.Shop.PostZone>>" %>
<%@ Import Namespace="Suteki.Common.ViewData"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div class="admin-form">
    <h1>Post Zone</h1>
    
    <%= Html.ValidationSummary()%>
    <%= Html.MessageBox(Model)%>

	<% using (Html.BeginForm())
	{ %>
		<p><%= this.TextBox(x => x.Item.Name).Label("Name")%></p>
		<p><%= this.TextBox(x => x.Item.Multiplier).Format("0.00").Label("Multiplier")%></p>
		<p><%= this.CheckBox(x => x.Item.AskIfMaxWeight).Label("Ask If Max Weight")%></p>
		<p><%= this.TextBox(x => x.Item.FlatRate).Format("0.00").Label("Flat Rate")%></p>
		<p><%= this.CheckBox(x => x.Item.IsActive).Label("Active")%></p>
        <p>
			<%= this.Hidden(x => x.Item.PostZoneId)%>
			<%= this.Hidden(x => x.Item.Position)%>
			<input type="submit" value="Save" />
        </p>
    <% } %>
</div>
</asp:Content>
