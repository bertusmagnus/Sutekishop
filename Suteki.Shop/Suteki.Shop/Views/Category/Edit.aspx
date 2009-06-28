<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

    <h1>Category</h1>
    
    <%= Html.ValidationSummary() %>
    <%= Html.MessageBox(ViewData.Model)%>

	<% using(Html.MultipartForm()) { %>
		<%= this.Hidden(x => x.Category.CategoryId) %>
		<%= this.Hidden(x => x.Category.Position) %>

		<%= this.TextBox(x => x.Category.Name).Label("Name") %>
		<%= this.Select(x => x.Category.ParentId).Options(Model.Categories, x => x.CategoryId, x => x.Name).Label("Parent Category") %>
        <%= this.CheckBox(x => x.Category.IsActive).Label("Active") %>
        
		<label for="image">Optional Image (For best results create an image 555 x 150 pixels)</label>
		
		<% if(Model.Category.ImageId  != null) { %>
            <%= Html.Image("~/ProductPhotos/" + Model.Category.Image.CategoryFileName) %><br />			
            <%= Html.ActionLink<CategoryController>(c => c.DeleteImage(Model.Category.CategoryId, Model.Category.ImageId.Value), "Delete Image") %>
		<% } %>
		
		<input type="file" name="image"  id="image" />
		
        <%= Html.SubmitButton() %>
    <% } %>
</asp:Content>