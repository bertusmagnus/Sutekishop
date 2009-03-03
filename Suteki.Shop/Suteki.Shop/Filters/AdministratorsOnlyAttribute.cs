using System.Web.Mvc;

namespace Suteki.Shop.Filters
{
	public class AdministratorsOnlyAttribute : AuthorizeAttribute
	{
		public AdministratorsOnlyAttribute()
		{
			Roles = "Administrator";
		}
	}
}