<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.User.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>User</h1>
    
    <%= Html.ErrorBox(ViewData) %>
    <%= Html.MessageBox(ViewData) %>

    <% using(Html.Form("User", "Update")) { %>

        <%= Html.Hidden("userid", ViewData.User.UserId) %>

        <label for="email">Email</label>
        <%= Html.TextBox("email", ViewData.User.Email) %>
        
        <label for="password">Password (leave blank if you don't want to change)</label>
        <%= Html.Password("password") %>

        <label for="roleid">Role</label>
        <%= Html.Select("roleid", ViewData.Roles, "Name", "RoleId", ViewData.User.RoleId)%>
        
        <label for="isenabled">User can log on</label>
        <%= Html.CheckBox("isenabled", "", "True", ViewData.User.IsEnabled) %>

        <%= Html.SubmitButton() %>

    <% } %>

</asp:Content>
