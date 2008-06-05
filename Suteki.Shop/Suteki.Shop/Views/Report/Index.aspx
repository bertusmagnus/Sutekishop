<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Index.aspx.cs" Inherits="Suteki.Shop.Views.Report.Index" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>Reports</h1>

<%= Html.ActionLink<ReportController>(c => c.Orders(), "Orders") %>

</asp:Content>
