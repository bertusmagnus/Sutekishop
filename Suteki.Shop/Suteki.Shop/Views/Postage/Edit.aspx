<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.Postage.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Postage Band</h1>
    
    <%= Html.ErrorBox(ViewData) %>
    <%= Html.MessageBox(ViewData) %>

    <% using (Html.Form("Postage", "Update"))
       { %>

        <%= Html.Hidden("postageid", ViewData.Postage.PostageId)%>
        <%= Html.Hidden("position", ViewData.Postage.Position)%>

        <label for="name">Name</label>
        <%= Html.TextBox("name", ViewData.Postage.Name)%>
        
        <label for="maxweight">Max Weight (grams)</label>
        <%= Html.TextBox("maxweight", ViewData.Postage.MaxWeight)%>
        
        <label for="price">Price</label>
        <%= Html.TextBox("price", ViewData.Postage.Price)%>
        
        <label for="isactive">Active</label>
        <%= Html.CheckBox("isactive", "", "true", ViewData.Postage.IsActive)%>
        
        <%= Html.SubmitButton()%>
        
    <% } %>

</asp:Content>
