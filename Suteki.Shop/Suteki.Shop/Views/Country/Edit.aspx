<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.Country.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Country</h1>
    
    <%= Html.ErrorBox(ViewData) %>
    <%= Html.MessageBox(ViewData) %>

    <% using (Html.Form("Country", "Update"))
       { %>

        <%= Html.Hidden("countryid", ViewData.Country.CountryId)%>
        <%= Html.Hidden("position", ViewData.Country.Position)%>

        <label for="name">Name</label>
        <%= Html.TextBox("name", ViewData.Country.Name)%>
        
        <label for="isactive">Active</label>
        <%= Html.CheckBox("isactive", "", "true", ViewData.Country.IsActive) %>
        
        <%= Html.SubmitButton()%>
        
    <% } %>

</asp:Content>
