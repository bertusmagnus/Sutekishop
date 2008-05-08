<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.Country.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Country</h1>
    
    <%= Html.ErrorBox(ViewData) %>
    <%= Html.MessageBox(ViewData) %>

    <% using (Html.Form("Country", "Update"))
       { %>

        <%= Html.Hidden("countryid", ViewData.Item.CountryId)%>
        <%= Html.Hidden("position", ViewData.Item.Position)%>

        <label for="name">Name</label>
        <%= Html.TextBox("name", ViewData.Item.Name)%>
        
        <label for="isactive">Active</label>
        <%= Html.CheckBox("isactive", "", "true", ViewData.Item.IsActive)%>
        
        <label for="postzoneid">Post Zone</label>
        <%= Html.Select("postzoneid", ViewData.GetLookUpList<PostZone>(), "Name", "PostZoneId", ViewData.Item.PostZoneId)%>
        
        <%= Html.SubmitButton()%>
        
    <% } %>

</asp:Content>
