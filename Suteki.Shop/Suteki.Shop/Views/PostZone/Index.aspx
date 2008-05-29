<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.PostZone.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>Postage Zones</h1>

<p><%= Html.ActionLink<PostZoneController>(c => c.New(), "New Postage Zone") %></p>

<table>
    <tr>
        <th class="thin">Name</th>
        <th class="thin number">Multiplier</th>
        <th class="wide">Ask If Max Weight</th>
        <th class="thin number">Flat Rate</th>
        <th class="thin">Active</th>
        <th>&nbsp;</th>
        <th>&nbsp;</th>
    </tr>
<% foreach (var postZone in ViewData.Model.Items)
   { %>
    <tr>
        <td><%= Html.ActionLink<PostZoneController>(c => c.Edit(postZone.PostZoneId), postZone.Name)%></td>
        <td class="number"><%= postZone.Multiplier.ToString("0.00##") %></td>
        <td><%= Html.Tick(postZone.AskIfMaxWeight)%></td>
        <td class="number"><%= postZone.FlatRate.ToString("£0.00") %></td>
        <td><%= Html.Tick(postZone.IsActive)%></td>
        <td><%= Html.UpArrowLink<PostZoneController>(c => c.MoveUp(postZone.Position))%></td>
        <td><%= Html.DownArrowLink<PostZoneController>(c => c.MoveDown(postZone.Position))%></td>
    </tr>
<% } %>
</table>


</asp:Content>
