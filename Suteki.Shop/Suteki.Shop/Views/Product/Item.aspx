<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.Master" Inherits="Suteki.Shop.ViewPage<ShopViewData>" %>
<%@ Import Namespace="Suteki.Common.Extensions"%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">

<script language="javascript">

function onThumbnailClick(img)
{
    var mainImage = document.getElementById("mainImage");
    var imgUrl = img.src.replace("thumb", "main");
    mainImage.src = imgUrl;
}

</script>

<div class="error"><%= TempData["message"] %></div>

<h1><%= ViewData.Model.Product.Name %>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<%= ViewData.Model.Product.Price.ToString("£0.00") %><%= ViewData.Model.Product.IsActiveAsString %></h1>

<% if(User.IsAdministrator()) { %>
    <p><%= Html.ActionLink<ProductController>(c => c.Edit(ViewData.Model.Product.ProductId), "Edit") %></p>
<% } %>

<% Html.RenderPartial("ProductDescription", Model.Product); %>
<% Html.RenderPartial("BasketOptions", Model.Product); %>

<p>If an item is out of stock, please email us at 
<a href="mailto:<%= ((Suteki.Shop.Controllers.ControllerBase)this.ViewContext.Controller).BaseControllerService.EmailAddress %>">
<%= ((Suteki.Shop.Controllers.ControllerBase)this.ViewContext.Controller).BaseControllerService.EmailAddress%>
</a>
 so that we can let you know when it will be available.</p>

</asp:Content>
