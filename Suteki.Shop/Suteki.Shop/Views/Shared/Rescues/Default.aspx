<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Suteki.Shop.Views.Shared.Rescue.Default" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<h1>An Error has occured in the application</h1>

<p>A record of the problem has been made. Please check back soon</p>

<!--

<%= ViewData.Model.GetType().Name %>

<%= ViewData.Model.Message %>

<%= ViewData.Model.StackTrace %>

<% if (ViewData.Model.InnerException != null)
   { %>

    <%= ViewData.Model.InnerException.GetType().Name %>

    <%= ViewData.Model.InnerException.Message %>

    <%= ViewData.Model.InnerException.StackTrace %>

    <% if (ViewData.Model.InnerException.InnerException != null)
       { %>

        <%= ViewData.Model.InnerException.InnerException.GetType().Name %>

        <%= ViewData.Model.InnerException.InnerException.Message %>

        <%= ViewData.Model.InnerException.InnerException.StackTrace %>

    <% } %>


<% } %>

-->

</asp:Content>
