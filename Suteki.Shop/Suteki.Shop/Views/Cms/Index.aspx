<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/CmsSubMenu.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.Cms.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <% if(User.IsInRole("Administrator")) { %>
        <p><%= Html.ActionLink<CmsController>(c => c.Edit(ViewData.TextContent.ContentId), "Edit") %></p>
    <% } %>

    <%= ViewData.TextContent.Text %>


</asp:Content>
