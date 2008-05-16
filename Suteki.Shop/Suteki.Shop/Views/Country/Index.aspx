<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.Country.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>Countries</h1>

<p><%= Html.ActionLink<CountryController>(c => c.New(), "New Country") %></p>

<table>
    <tr>
        <th class="wide">Name</th>
        <th class="thin">Active</th>
        <th class="thin">Post Zone</th>
        <th>&nbsp;</th>
        <th>&nbsp;</th>
    </tr>
<% foreach(var country in ViewData.Items) { %>
    <tr>
        <td><%= Html.ActionLink<CountryController>(c => c.Edit(country.CountryId), country.Name) %></td>
        <td><%= Html.Tick(country.IsActive) %></td>
        <td><%= country.PostZone.Name %></td>
        <td><%= Html.UpArrowLink<CountryController>(c => c.MoveUp(country.Position)) %></td>
        <td><%= Html.DownArrowLink<CountryController>(c => c.MoveDown(country.Position)) %></td>
    </tr>
<% } %>
</table>

</asp:Content>
