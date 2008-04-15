<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.User.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <p><%= Html.ActionLink<UserController>(c => c.New(), "New User") %></p>

    <table>
        <tr>
            <th>Email</th>
            <th>Role</th>
        </tr>
        
        <% foreach (var user in ViewData.Users)
           { %>
            
            <tr>
                <td><%= Html.ActionLink<UserController>(c => c.Edit(user.UserId), user.Email) %></td>
                <td><%= user.Role.Name %></td>
            </tr>
            
        <% } %>
    </table>

</asp:Content>
