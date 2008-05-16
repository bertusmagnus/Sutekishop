<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.Postage.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Postage Band</h1>
    
    <%= Html.ErrorBox(ViewData) %>
    <%= Html.MessageBox(ViewData) %>

    <% using (Html.Form("Postage", "Update"))
       { %>

        <%= Html.Hidden("postageid", ViewData.Item.PostageId)%>
        <%= Html.Hidden("position", ViewData.Item.Position)%>

        <label for="name">Name</label>
        <%= Html.TextBox("name", ViewData.Item.Name)%>
        
        <label for="maxweight">Max Weight (grams)</label>
        <%= Html.TextBox("maxweight", ViewData.Item.MaxWeight.ToString())%>
        
        <label for="price">Price</label>
        <%= Html.TextBox("price", ViewData.Item.Price)%>
        
        <label for="isactive">Active</label>
        <%= Html.CheckBox("isactive", "", "true", ViewData.Item.IsActive)%>
        
        <%= Html.SubmitButton()%>
        
    <% } %>

</asp:Content>
