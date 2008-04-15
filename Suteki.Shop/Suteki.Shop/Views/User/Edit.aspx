<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.User.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>User</h1>
    
    <%= Html.ErrorBox(ViewData) %>
    <%= Html.MessageBox(ViewData) %>

    <% using(Html.Form("User", "Update")) { %>

        <%= Html.Hidden("userid", ViewData.User.UserId) %>

        <label for="email">Email</label>
        <%= Html.TextBox("email", ViewData.User.Email) %>
        
        <label for="password">Password</label>
        <%= Html.Password("password") %>

        <label for="roleid">Role</label>
        <%= Html.Select("roleid", ViewData.Roles, "Name", "RoleId", ViewData.User.RoleId)%>

        <%= Html.SubmitButton() %>

    <% } %>

</asp:Content>
