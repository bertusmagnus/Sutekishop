<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/CmsSubMenu.master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.Cms.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>Content</h1>

<p><%= ViewData.Model.Content.Link(Html)%></p>

<%= Html.MessageBox(ViewData.Model)%>
<%= Html.ErrorBox(ViewData.Model)%>

<% using (Html.BeginForm<CmsController>(c => c.Update(ViewData.Model.Content.ContentId)))
   { %>

    <%= Html.Hidden("contenttypeid", ViewData.Model.Content.ContentTypeId.ToString())%>
    <%= Html.Hidden("position", ViewData.Model.Content.Position.ToString())%>

    <label for="name">Name</label>
    <%= Html.TextBox("name", ViewData.Model.Content.Name)%>

    <% if (ViewData.Model.Content.IsTextContent)
       { %>

        <label for="text">Text</label>
        <%= Html.TextArea("text", ViewData.Model.TextContent.Text)%>
    
    <% } %>

    <label for="parentcontentid">Parent Menu</label>
    <%= Html.DropDownList("parentcontentid", new SelectList(ViewData.Model.Menus, "ContentId", "Name", ViewData.Model.Content.ParentContentId))%>

    <label for="isactive">Active</label>
    <%= Html.CheckBox("isactive", ViewData.Model.Content.IsActive)%>

    <%= Html.SubmitButton() %>

<% } %>

</asp:Content>
