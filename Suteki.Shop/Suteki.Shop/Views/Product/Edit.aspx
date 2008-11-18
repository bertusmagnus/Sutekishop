<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" AutoEventWireup="true" ValidateRequest="false" CodeBehind="Edit.aspx.cs" Inherits="Suteki.Shop.Views.Product.Edit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Product</h1>
    
    <% if(ViewData.Model.Product.ProductId > 0) { %>
        <%= Html.ActionLink<ProductController>(c => c.Item(ViewData.Model.Product.UrlName), "Preview") %>
    <% } %>

    <%= Html.ErrorBox(ViewData.Model) %>
    <%= Html.MessageBox(ViewData.Model) %>

    <% using (Html.BeginForm("Product", "Update", FormMethod.Post, new Dictionary<string, object> { { "enctype", "multipart/form-data" } } ))
       { %>

        <%= Html.Hidden("productId", ViewData.Model.Product.ProductId.ToString())%>
        <%= Html.Hidden("position", ViewData.Model.Product.Position.ToString())%>

        <div class="columnContainer">
            <div class="contentLeftColumn">
                <label for="name">Name</label>
                <%= Html.TextBox("name", ViewData.Model.Product.Name)%>
                
                <label for="categoryid">Category</label>
                <%= Html.DropDownList("categoryid", new SelectList(ViewData.Model.Categories, "CategoryId", "Name", ViewData.Model.Product.CategoryId))%>
                
                <label for="weight">Weight</label>
                <%= Html.TextBox("weight", ViewData.Model.Product.Weight.ToString())%>
                
                <label for="price">Price £</label>
                <%= Html.TextBox("price", ViewData.Model.Product.Price.ToString("0.00"))%>
                
                <label for="isactive">Active</label>
                <%= Html.CheckBox("isactive", ViewData.Model.Product.IsActive)%>
            </div>
            
            <div class="contentRightColumn">
                <br /><br /><br /><br /><br /><br /><br /><br /><br />
            </div>
        </div>
        
        <label for="description">Description</label>
        <%= Html.TextArea("description", ViewData.Model.Product.Description)%>
        
        <h3>Sizes</h3>
        
        <p>
        <% foreach(var size in ViewData.Model.Product.Sizes.Active()) { %>
            <%= size.Name %>&nbsp;
        <% } %>
        
        <%= Html.ActionLink<ProductController>(c => c.ClearSizes(ViewData.Model.Product.ProductId), "Clear all sizes")%>
        </p>
        <div class="sizeInput">
        <% for(int i=0; i<10; i++) { %>
            <%= Html.TextBox("size_" + i.ToString())%>
        <% } %>
        </div>
        
        <h3>Photos</h3>
        
        <div class="imageList">
        <% foreach(var productImage in ViewData.Model.Product.ProductImages.InOrder()) { %>
            <div class="imageEdit">
            <%= Html.Image("~/ProductPhotos/" + productImage.Image.ThumbFileName) %><br />
            <%= Html.UpArrowLink<ProductController>(c => c.MoveImageUp(ViewData.Model.Product.ProductId, productImage.Position)) %>
            <%= Html.DownArrowLink<ProductController>(c => c.MoveImageDown(ViewData.Model.Product.ProductId, productImage.Position)) %> &nbsp;&nbsp;
            <%= Html.CrossLink<ProductController>(c => c.DeleteImage(ViewData.Model.Product.ProductId, productImage.ProductImageId)) %>
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
