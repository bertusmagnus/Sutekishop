<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.PostZone.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Post Zone</h1>
    
    <%= Html.ErrorBox(ViewData.Model) %>
    <%= Html.MessageBox(ViewData.Model) %>

    <% using (Html.BeginForm("PostZone", "Update"))
       { %>

        <%= Html.Hidden("postzoneid", ViewData.Model.Item.PostZoneId.ToString())%>
        <%= Html.Hidden("position", ViewData.Model.Item.Position.ToString())%>

        <label for="name">Name</label>
        <%= Html.TextBox("name", ViewData.Model.Item.Name)%>
        
        <label for="multiplier">Multiplier</label>
        <%= Html.TextBox("multiplier", ViewData.Model.Item.Multiplier.ToString())%>
        
        <label for="askifmaxweight">Ask If Max Weight</label>
        <%= Html.CheckBox("askifmaxweight", ViewData.Model.Item.AskIfMaxWeight)%>
        
        <label for="flatrate">Flat Rate</label>
        <%= Html.TextBox("flatrate", ViewData.Model.Item.FlatRate.ToString("0.00"))%>
        
        <label for="isactive">Active</label>
        <%= Html.CheckBox("isactive", ViewData.Model.Item.IsActive)%>
        
        <%= Html.SubmitButton()%>
        
    <% } %>


</asp:Content>
