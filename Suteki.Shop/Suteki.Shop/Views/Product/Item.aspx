<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<%@ Import Namespace="Suteki.Common.Extensions"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
<div class="error"><%= TempData["message"] %></div>

<% if(User.IsAdministrator()) { %>
    <ul id="admin-submenu">
		<li><%= Html.ActionLink<ProductController>(c => c.Edit(Model.Product.ProductId), "Edit") %></li>
	</ul>
<% } %>

<h1><%= Html.Encode(ViewData.Model.Product.Name) %>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%= ViewData.Model.Product.Price.ToString("£0.00") %><%= ViewData.Model.Product.IsActiveAsString %></h1>

<% Html.RenderPartial("ProductDescription", Model.Product); %>
<% Html.RenderPartial("BasketOptions", Model.Product); %>

<p id="out-of-stock-helper">If an item is out of stock, please email us at 
<a href="mailto:<%= ((Suteki.Shop.Controllers.ControllerBase)this.ViewContext.Controller).BaseControllerService.EmailAddress %>">
<%= ((Suteki.Shop.Controllers.ControllerBase)this.ViewContext.Controller).BaseControllerService.EmailAddress%>
</a>
 so that we can let you know when it will be available.</p>

<% Html.RenderAction<ReviewsController>(c => c.Show(Model.Product.ProductId)); %>


</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="Scripts" runat="server">
<script type="text/javascript">
$(document).ready(function(){
	$('.imageList a').click(function(){
		var mainImage = $("#mainImage")[0];
		var imgUrl = $(this).find('img').attr('src').replace("thumb", "main");
		mainImage.src = imgUrl;
	});
});
</script>
</asp:Content>