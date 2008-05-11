<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.PostZone.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Post Zone</h1>
    
    <%= Html.ErrorBox(ViewData) %>
    <%= Html.MessageBox(ViewData) %>

    <% using (Html.Form("PostZone", "Update"))
       { %>

        <%= Html.Hidden("postzoneid", ViewData.Item.PostZoneId)%>
        <%= Html.Hidden("position", ViewData.Item.Position)%>

        <label for="name">Name</label>
        <%= Html.TextBox("name", ViewData.Item.Name)%>
        
        <label for="multiplier">Multiplier</label>
        <%= Html.TextBox("multiplier", ViewData.Item.Multiplier.ToString())%>
        
        <label for="askifmaxweight">Ask If Max Weight</label>
        <%= Html.CheckBox("askifmaxweight", "", "true", ViewData.Item.AskIfMaxWeight)%>
        
        <label for="flatrate">Flat Rate</label>
        <%= Html.TextBox("flatrate", ViewData.Item.FlatRate.ToString("0.00"))%>
        
        <label for="isactive">Active</label>
        <%= Html.CheckBox("isactive", "", "true", ViewData.Item.IsActive)%>
        
        <%= Html.SubmitButton()%>
        
    <% } %>


</asp:Content>
