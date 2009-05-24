using Suteki.Common.Validation;

namespace Suteki.Shop
{
	public partial class MailingListSubscription
	{
		partial void OnEmailChanging(string value) 
		{
			value.Label("Email").IsRequired().WithMaxLength(250);
		}
	}
}