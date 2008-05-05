<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.Postage.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>Postage Bands</h1>

<p><%= Html.ActionLink<PostageController>(c => c.New(), "New Postage Band") %></p>

<table>
    <tr>
        <th class="thin">Name</th>
        <th class="thin number">Max Weight</th>
        <th class="thin number">Price</th>
        <th class="thin">Active</th>
        <th>&nbsp;</th>
        <th>&nbsp;</th>
    </tr>
<% foreach(var postage in ViewData.Items) { %>
    <tr>
        <td><%= Html.ActionLink<PostageController>(c => c.Edit(postage.PostageId), postage.Name)%></td>
        <td class="number"><%= postage.MaxWeight.ToString() %></td>
        <td class="number"><%= postage.Price.ToString("£0.00") %></td>
        <td><%= Html.Tick(postage.IsActive)%></td>
        <td><%= Html.UpArrowLink<PostageController>(c => c.MoveUp(postage.Position))%></td>
        <td><%= Html.DownArrowLink<PostageController>(c => c.MoveDown(postage.Position))%></td>
    </tr>
<% } %>
</table>


</asp:Content>
