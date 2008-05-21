<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Cms.master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.Cms.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>Text Content</h1>

<% using(Html.Form<CmsController>(c => c.Update(ViewData.TextContent.ContentId))) { %>

    <%= Html.Hidden("menuid", ViewData.TextContent.MenuId) %>
    <%= Html.Hidden("contenttypeid", ViewData.TextContent.ContentTypeId)%>
    <%= Html.Hidden("position", ViewData.TextContent.Position) %>

    <label for="name">Name</label>
    <%= Html.TextBox("name", ViewData.TextContent.Name) %>

    <label for="text">Text</label>
    <%= Html.TextArea("text", ViewData.TextContent.Text) %>

    <label for="isactive">Active</label>
    <%= Html.CheckBox("isactive", "", "True", ViewData.TextContent.IsActive)%>

    <%= Html.SubmitButton() %>

<% } %>

</asp:Content>
