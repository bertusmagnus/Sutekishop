<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.User.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>User</h1>
    
    <%= Html.ErrorBox(ViewData.Model) %>
    <%= Html.MessageBox(ViewData.Model) %>

    <% using(Html.BeginForm("Update", "User")) { %>

        <%= Html.Hidden("userid", ViewData.Model.User.UserId.ToString()) %>

        <label for="email">Email</label>
        <%= Html.TextBox("email", ViewData.Model.User.Email) %>
        
        <label for="password">Password (leave blank if you don't want to change)</label>
        <%= Html.Password("password") %>

        <label for="roleid">Role</label>
        <%= Html.DropDownList("roleid", new SelectList(ViewData.Model.Roles, "RoleId", "Name", ViewData.Model.User.RoleId))%>
        
        <label for="isenabled">User can log on</label>
        <%= Html.CheckBox("isenabled", ViewData.Model.User.IsEnabled) %>

        <%= Html.SubmitButton() %>

    <% } %>

</asp:Content>
