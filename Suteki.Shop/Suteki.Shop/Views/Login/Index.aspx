<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.Login.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Login</h1>

    <%= Html.ErrorBox(ViewData.Model)%>

    <form method="post" action="<%= Url.Action("Authenticate", "Login").ToSslUrl() %>">

        <label for="username">Email</label>
        <%= Html.TextBox("email") %>
        
        <label for="password">Password</label>
        <%= Html.Password("password") %>

        <%= Html.SubmitButton() %>

    </form>


</asp:Content>
