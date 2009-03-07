<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" Inherits="Suteki.Shop.ViewPage<ScaffoldViewData<Suteki.Shop.PostZone>>" %>
<%@ Import Namespace="Suteki.Common.ViewData"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>Postage Zones</h1>

<p><%= Html.ActionLink<PostZoneController>(c => c.New(), "New Postage Zone") %></p>

<%= Html.Grid(Model.Items).Columns(column => {
		column.For(x => Html.ActionLink<PostZoneController>(c => c.Edit(x.PostZoneId), x.Name)).Named("Name").DoNotEncode().HeaderAttributes(@class => "thin");
		column.For(x => x.Multiplier.ToString("0.00##")).Named("Multiplier").Attributes(@class => "number").HeaderAttributes(@class => "thin number");
		column.For(x => Html.Tick(x.AskIfMaxWeight)).DoNotEncode().Named("Ask If Max Weight").HeaderAttributes(@class => "wide");
		column.For(x => x.FlatRate).Format("{0:C}").HeaderAttributes(@class => "thin number").Attributes(@class => "number");
		column.For(x => Html.Tick(x.IsActive)).Named("Active").DoNotEncode().HeaderAttributes(@class => "thin");
		column.For(x => Html.UpArrowLink<PostZoneController>(c => c.MoveUp(x.Position, 1))).DoNotEncode().Named("&nbsp;");
		column.For(x => Html.DownArrowLink<PostZoneController>(c => c.MoveDown(x.Position, 1))).DoNotEncode().Named("&nbsp;");
   }) %>
</asp:Content>
