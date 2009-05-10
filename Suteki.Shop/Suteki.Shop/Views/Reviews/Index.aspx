<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Shop.master" Inherits="System.Web.Mvc.ViewPage<ReviewViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <h2>Unapproved Reviews</h2>
    <% foreach(var review in Model.Reviews) { %>
    <div>
		<p>
			<strong>Product:</strong> <%= Html.Encode(review.Product.Name) %>
		</p>
		<p>
			<strong>Reviewer:</strong> <%= Html.Encode(review.Reviewer) %>
		</p>
		<p>
			<strong>Rating:</strong> <%= Html.Stars(review.Rating) %>
		</p>
		<p>
			<%= Html.Encode(review.Text) %>
		</p>
		
		<% using (Html.BeginForm<ReviewsController>(c => c.Approve(review.Id))) { %>
			<input type="submit" value="Approve" />
		<% } %>
		
		<% using (Html.BeginForm<ReviewsController>(c => c.Delete(review.Id))) { %>
			<input type="submit" value="Delete" />
		<% } %>
		
		<hr />
	</div>
    <% } %>
    
    <% if (Model.Reviews.Count() == 0) { %>
		<p>There are no outstanding reviews.</p>
    <% } %>
</asp:Content>