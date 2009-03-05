<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Category</h1>
    
    <%= Html.ErrorBox(ViewData.Model)%>
    <%= Html.MessageBox(ViewData.Model)%>

    <% using (Html.BeginForm("Update", "Category"))
       { %>

        <%= Html.Hidden("categoryId", ViewData.Model.Category.CategoryId.ToString())%>
        <%= Html.Hidden("position", ViewData.Model.Category.Position.ToString())%>

        <label for="name">Name</label>
        <%= Html.TextBox("name", ViewData.Model.Category.Name)%>
        
        <label for="parentid">Parent Category</label>
        <%= Html.DropDownList("parentid", new SelectList(ViewData.Model.Categories, "CategoryId", "Name", ViewData.Model.Category.ParentId))%>
        
        <label for="isactive">Active</label>
        <%= Html.CheckBox("isactive", ViewData.Model.Category.IsActive)%>
        
        <%= Html.SubmitButton() %>

    <% } %>

</asp:Content>
