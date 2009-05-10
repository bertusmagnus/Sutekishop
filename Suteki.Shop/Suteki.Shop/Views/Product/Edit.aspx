<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <h1>Product</h1>
    
	<%= Html.ValidationSummary() %>
    <%= Html.MessageBox(Model) %>
    
    <% if(Model.Product.ProductId > 0) { %>
        <%= Html.ActionLink<ProductController>(c => c.Item(Model.Product.UrlName), "Preview") %>
    <% } %>

    <% using (Html.MultipartForm()) { %>
		<%= this.Hidden(x => x.Product.ProductId) %>
		<%= this.Hidden(x => x.Product.Position) %>

		<p><%= this.TextBox(x => x.Product.Name).Label("Name") %></p>
		<p><%= this.MultiSelect(x => x.Product.ProductCategories.Select(y => y.CategoryId)).Name("categories").Options(Model.Categories, x => x.CategoryId, x => x.Name).Label("Categories (ctrl+click to select more than one)") %></p>
        <p><%= this.TextBox(x => x.Product.Weight).Label("Weight") %></p>
        <p><%= this.TextBox(x => x.Product.Price).Format("{0:0.00}").Label("Price £") %></p>
        <p><%= this.CheckBox(x => x.Product.IsActive).Label("Active") %></p>    
        <p><%= this.TextArea(x => x.Product.Description).Label("Description") %></p>
        
        <h3>Sizes</h3>
        
        <p>
        <% foreach(var size in Model.Product.Sizes.Active()) { %>
            <%= size.Name %>&nbsp;
        <% } %>
        
        <%= Html.ActionLink<ProductController>(c => c.ClearSizes(Model.Product.ProductId), "Clear all sizes")%>
        </p>
        <div class="sizeInput">
        <% for(int i=0; i<10; i++) { %>
            <p><%= Html.TextBox("size_" + i)%></p>
        <% } %>
        </div>
        
        <h3>Photos</h3>
        
        <div class="imageList">
        <% foreach(var productImage in Model.Product.ProductImages.InOrder()) { %>
            <div class="imageEdit">
            <%= Html.Image("~/ProductPhotos/" + productImage.Image.ThumbFileName) %><br />
            <%= Html.UpArrowLink<ProductImageController>(c => c.MoveImageUp(Model.Product.ProductId, productImage.Position)) %>
            <%= Html.DownArrowLink<ProductImageController>(c => c.MoveImageDown(Model.Product.ProductId, productImage.Position)) %> &nbsp;&nbsp;
            <%= Html.CrossLink<ProductImageController>(c => c.DeleteImage(Model.Product.ProductId, productImage.ProductImageId)) %>
            </div>
        <% } %>
        </div>
        
        <div class="clear" />
        
        <% for (int i = 0; i < 5; i++) { %>
            <p><input type="file" id="image_<%= i.ToString() %>" name="image_<%= i.ToString() %>" /></p>
        <% } %>
        
        <p><input type="submit" value="Save Changes" /></p>
    <% } %>
    
    <% Html.InitialiseRichTextEditor(); %>
    
</asp:Content>