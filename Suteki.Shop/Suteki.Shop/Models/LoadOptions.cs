using System;
using System.Data.Linq;

namespace Suteki.Shop.Models
{
	public interface ILoadOptions
	{
		void Build(DataLoadOptions options);
	}

	public class ReviewsWithProducts : ILoadOptions
	{
		public void Build(DataLoadOptions options)
		{
			options.LoadWith<Review>(x => x.Product);
		}
	}

	public class MailingListSubscriptionsWithCountries : ILoadOptions
	{
		public void Build(DataLoadOptions options)
		{
			options.LoadWith<MailingListSubscription>(x => x.Contact);
			options.LoadWith<Contact>(x => x.County);
		}
	}
}