<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.Country.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Country</h1>
    
    <%= Html.ErrorBox(ViewData.Model)%>
    <%= Html.MessageBox(ViewData.Model)%>

    <% using (Html.BeginForm("Update", "Country"))
       { %>

        <%= Html.Hidden("countryid", ViewData.Model.Item.CountryId.ToString())%>
        <%= Html.Hidden("position", ViewData.Model.Item.Position.ToString())%>

        <label for="name">Name</label>
        <%= Html.TextBox("name", ViewData.Model.Item.Name)%>
        
        <label for="isactive">Active</label>
        <%= Html.CheckBox("isactive", ViewData.Model.Item.IsActive)%>
        
        <label for="postzoneid">Post Zone</label>
        <%= Html.DropDownList("postzoneid", new SelectList(ViewData.Model.GetLookupList(typeof(PostZone)), "PostZoneId", "Name", ViewData.Model.Item.PostZoneId))%>
        
        <%= Html.SubmitButton()%>
        
    <% } %>

</asp:Content>
