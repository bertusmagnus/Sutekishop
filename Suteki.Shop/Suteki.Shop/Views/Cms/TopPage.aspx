<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Cms.master" Inherits="Suteki.Shop.ViewPage<CmsViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <div id="cms-content">
		<%= Model.TextContent.Text%>
		
		 <% if(User.IsAdministrator()) { %>
			<p><%= Html.ActionLink<CmsController>(c => c.Edit(Model.Content.ContentId), "Edit")%></p>
		<% } %>
    </div>
</asp:Content>