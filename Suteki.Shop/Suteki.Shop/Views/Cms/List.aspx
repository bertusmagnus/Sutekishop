<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Cms.master" AutoEventWireup="true" CodeBehind="List.aspx.cs" Inherits="Suteki.Shop.Views.Cms.List" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<%= Html.ActionLink<CmsController>(c => c.Add(ViewData.Menu.MenuId), "New Page") %>

<ul>
<% foreach(var content in ViewData.Menu.Contents.InOrder()) { %>
    <li>
    <%= content.Name %>
    <%= Html.ActionLink<CmsController>(c => c.Edit(content.ContentId), "Edit") %>
    <%= Html.UpArrowLink<CmsController>(c => c.MoveUp(content.ContentId)) %>
    <%= Html.DownArrowLink<CmsController>(c => c.MoveDown(content.ContentId))%>
    </li>
<% } %>
</ul>

</asp:Content>
