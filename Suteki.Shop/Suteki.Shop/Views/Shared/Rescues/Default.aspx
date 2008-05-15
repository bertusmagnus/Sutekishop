<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Suteki.Shop.Views.Shared.Rescue.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>An Error has occured in the application</h1>

<p>A record of the problem has been made. Please check back soon</p>

<!--

<%= ViewData.GetType().Name %>

<%= ViewData.Message %>

<%= ViewData.StackTrace %>

<% if (ViewData.InnerException != null)
   { %>

    <%= ViewData.InnerException.GetType().Name %>

    <%= ViewData.InnerException.Message %>

    <%= ViewData.InnerException.StackTrace %>

    <% if (ViewData.InnerException.InnerException != null)
       { %>

        <%= ViewData.InnerException.InnerException.GetType().Name %>

        <%= ViewData.InnerException.InnerException.Message %>

        <%= ViewData.InnerException.InnerException.StackTrace %>

    <% } %>


<% } %>

-->

</asp:Content>
