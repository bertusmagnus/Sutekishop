<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.Product.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Product</h1>
    
    <% if(ViewData.Product.ProductId > 0) { %>
        <%= Html.ActionLink<ProductController>(c => c.Item(ViewData.Product.ProductId), "Preview") %>
    <% } %>

    <%= Html.ErrorBox(ViewData) %>
    <%= Html.MessageBox(ViewData) %>

    <% using (Html.Form("Product", "Update", FormMethod.Post, new Dictionary<string, object> { { "enctype", "multipart/form-data" } } ))
       { %>

        <%= Html.Hidden("productId", ViewData.Product.ProductId)%>
        <%= Html.Hidden("position", ViewData.Product.Position)%>

        <div class="columnContainer">
            <div class="contentLeftColumn">
                <label for="name">Name</label>
                <%= Html.TextBox("name", ViewData.Product.Name)%>
                
                <label for="categoryid">Category</label>
                <%= Html.Select("categoryid", ViewData.Categories, "Name", "CategoryId", ViewData.Product.CategoryId)%>
                
                <label for="weight">Weight</label>
                <%= Html.TextBox("weight", ViewData.Product.Weight.ToString())%>
                
                <label for="price">Price £</label>
                <%= Html.TextBox("price", ViewData.Product.Price.ToString("0.00"))%>
                
                <label for="isactive">Active</label>
                <%= Html.CheckBox("isactive", "", "True", ViewData.Product.IsActive)%>
            </div>
        </div>
        
        <label for="description">Description</label>
        <%= Html.TextArea("description", ViewData.Product.Description)%>
        
        <h3>Sizes</h3>
        
        <p>
        <% foreach(var size in ViewData.Product.Sizes.Active()) { %>
            <%= size.Name %>&nbsp;
        <% } %>
        
        <%= Html.ActionLink<ProductController>(c => c.ClearSizes(ViewData.Product.ProductId), "Clear all sizes")%>
        </p>
        <div class="sizeInput">
        <% for(int i=0; i<10; i++) { %>
            <%= Html.TextBox("size_" + i.ToString(), null, 10, 10, new { _class = "inline" })%>
        <% } %>
        </div>
        
        <h3>Photos</h3>
        
        <div class="imageList">
        <% foreach(var productImage in ViewData.Product.ProductImages.InOrder()) { %>
            <div class="imageEdit">
            <%= Html.Image("~/ProductPhotos/" + productImage.Image.ThumbFileName) %><br />
            <%= Html.UpArrowLink<ProductController>(c => c.MoveImageUp(ViewData.Product.ProductId, productImage.Position)) %>
            <%= Html.DownArrowLink<ProductController>(c => c.MoveImageDown(ViewData.Product.ProductId, productImage.Position)) %> &nbsp;&nbsp;
            <%= Html.CrossLink<ProductController>(c => c.DeleteImage(ViewData.Product.ProductId, productImage.ProductImageId)) %>
            </div>
        <% } %>
        </div>
        
        <div class="clear" />
        
        <% for (int i = 0; i < 5; i++)
           { %>
            <input type="file" id="image_<%= i.ToString() %>" name="image_<%= i.ToString() %>" />
        <% } %>
        <%= Html.SubmitButton()%>

    <% } %>


</asp:Content>
