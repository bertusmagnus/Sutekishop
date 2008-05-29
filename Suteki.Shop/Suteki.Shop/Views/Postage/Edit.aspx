<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.Postage.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Postage Band</h1>
    
    <%= Html.ErrorBox(ViewData.Model) %>
    <%= Html.MessageBox(ViewData.Model) %>

    <% using (Html.Form("Postage", "Update"))
       { %>

        <%= Html.Hidden("postageid", ViewData.Model.Item.PostageId.ToString())%>
        <%= Html.Hidden("position", ViewData.Model.Item.Position.ToString())%>

        <label for="name">Name</label>
        <%= Html.TextBox("name", ViewData.Model.Item.Name)%>
        
        <label for="maxweight">Max Weight (grams)</label>
        <%= Html.TextBox("maxweight", ViewData.Model.Item.MaxWeight.ToString())%>
        
        <label for="price">Price</label>
        <%= Html.TextBox("price", ViewData.Model.Item.Price)%>
        
        <label for="isactive">Active</label>
        <%= Html.CheckBox("isactive", "", "true", ViewData.Model.Item.IsActive)%>
        
        <%= Html.SubmitButton()%>
        
    <% } %>

</asp:Content>
