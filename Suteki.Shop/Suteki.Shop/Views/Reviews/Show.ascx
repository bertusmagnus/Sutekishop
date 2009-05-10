<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ReviewViewData>" %>

<%= Html.ActionLink<ReviewsController>(c => c.New(Model.ProductId), "Leave a Review") %>

<% if (Model.Reviews.Count() > 0) { %>

<a id="show-reviews" href="javascript:void(0)">Show Reviews</a>

<div id="reviews" style="display: none">
	<% foreach (var review in Model.Reviews) { %>
		<p>
			<%= Html.Stars(review.Rating) %> <%= Html.Encode(review.Reviewer) %>
		</p>
		<p>
			<%= Html.Encode(review.Text) %>
		</p>
	<% } %>
</div>

<script type="text/javascript">
	$(function() {
		$('#show-reviews').click(function() {
			$('#reviews').toggle('slide');
		});
	});
</script>

<% } %>