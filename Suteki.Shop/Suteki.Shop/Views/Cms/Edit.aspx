<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/CmsSubMenu.master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.Cms.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>Content</h1>

<p><%= ViewData.Content.Link(Html) %></p>

<%= Html.MessageBox(ViewData) %>
<%= Html.ErrorBox(ViewData) %>

<% using(Html.Form<CmsController>(c => c.Update(ViewData.Content.ContentId))) { %>

    <%= Html.Hidden("parentcontentid", ViewData.Content.ParentContentId)%>
    <%= Html.Hidden("contenttypeid", ViewData.Content.ContentTypeId)%>
    <%= Html.Hidden("position", ViewData.Content.Position) %>

    <label for="name">Name</label>
    <%= Html.TextBox("name", ViewData.Content.Name) %>

    <% if(ViewData.Content.IsTextContent) { %>

        <label for="text">Text</label>
        <%= Html.TextArea("text", ViewData.TextContent.Text) %>
    
    <% } %>

    <label for="isactive">Active</label>
    <%= Html.CheckBox("isactive", "", "True", ViewData.Content.IsActive)%>

    <%= Html.SubmitButton() %>

<% } %>

</asp:Content>
