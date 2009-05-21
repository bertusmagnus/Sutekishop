<%@ Control Language="C#" Inherits="System.Web.Mvc.ViewUserControl<ReviewViewData>" %>

<%= Html.ActionLink<ReviewsController>(c => c.New(Model.Product.ProductId), "Leave a Review") %>

<% if (Model.Reviews.Count() > 0) { %>

<a id="show-reviews" href="javascript:void(0)">Show Reviews</a>

<div id="reviews" style="display: none;">
	<p>&nbsp;</p>
	<% foreach (var review in Model.Reviews) { %>
		<div>
			<%= Html.Stars(review.Rating) %> <%= Html.Encode(review.Reviewer) %>
			<% if(Context.User.IsAdministrator()) { %>
				<% using(Html.BeginForm<ReviewsController>(c => c.Delete(review.Id), FormMethod.Post, new{ style="display:inline;" })) { %>
					<img src="<%= Url.Content("~/Content/Images/cross.png") %>" alt="delete" class="delete-review pointer" />
				<% } %>
			<% } %>
		</div>
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
		$('.delete-review').click(function() {
			if (confirm('Are you sure you want to delete this review?')) {
				$(this).parent().submit();
			}
		});
	});
</script>

<% } %>