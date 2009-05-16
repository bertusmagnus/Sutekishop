<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="Suteki.Shop.ViewPage<ReviewViewData>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContentPlaceHolder" runat="server">
    <h2>Add a Review for <%= Html.Encode(Model.Product.Name) %></h2>
	<%= Html.ValidationSummary() %>
	<% using (Html.BeginForm()) { %>
		<p>Your Name:</p>
		<p>
			<%= this.TextBox(x => x.Review.Reviewer).MaxLength(250) %>
		</p>
		<p>Product Review:</p>
		<p><%= this.TextArea(x => x.Review.Text).Rows(10).Columns(40) %></p>
		<%= this.Hidden(x => x.Product.ProductId).Name("id") %>
		<p>Rating:</p>
		<input name="<%= this.NameFor(x => x.Review.Rating) %>" type="radio" value="5" /> <%= Html.Stars(5) %><br />
		<input name="<%= this.NameFor(x => x.Review.Rating) %>" type="radio" value="4" /> <%= Html.Stars(4) %><br />
		<input name="<%= this.NameFor(x => x.Review.Rating) %>" type="radio" value="3" checked="checked" /> <%= Html.Stars(3) %><br />
		<input name="<%= this.NameFor(x => x.Review.Rating) %>" type="radio" value="2" /> <%= Html.Stars(2) %><br />
		<input name="<%= this.NameFor(x => x.Review.Rating) %>" type="radio" value="1" /> <%= Html.Stars(1) %><br />
		<input type="submit" value="Submit Review" />
	<% } %>

</asp:Content>
