using System;
using System.Collections.Generic;
using System.Linq;

namespace Suteki.Shop.ViewData
{
	public class ReviewViewData
	{
		public IEnumerable<Review> Reviews { get; set; }

		//public int ProductId { get; set; }

		public Review Review { get; set; }

		public Product Product { get; set; }
	}
}