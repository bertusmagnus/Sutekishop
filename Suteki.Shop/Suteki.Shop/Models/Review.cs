using Suteki.Common.Validation;

namespace Suteki.Shop
{
	public partial class Review
	{
		partial void OnReviewerChanging(string value)
		{
			value.Label("Reviewer Name").IsRequired();
		}

		partial void OnTextChanging(string value)
		{
			value.Label("Review").IsRequired();
		}
	}
}