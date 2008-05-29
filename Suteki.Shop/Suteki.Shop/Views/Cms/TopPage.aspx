<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Cms.master" AutoEventWireup="true" CodeBehind="TopPage.aspx.cs" Inherits="Suteki.Shop.Views.Cms.TopPage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <%= ViewData.Model.TextContent.Text%>

    <% if(User.IsInRole("Administrator")) { %>
        <p><%= Html.ActionLink<CmsController>(c => c.Edit(ViewData.Model.Content.ContentId), "Edit")%></p>
    <% } %>

</asp:Content>
