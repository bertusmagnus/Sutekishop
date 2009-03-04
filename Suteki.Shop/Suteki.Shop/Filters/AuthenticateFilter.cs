using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Shop.Repositories;
using Suteki.Shop.Services;

namespace Suteki.Shop.Filters
{
	public class AuthenticateAttribute : FilterUsingAttribute
	{
		public AuthenticateAttribute() : base(typeof(AuthenticateFilter))
		{
		}
	}

	public class AuthenticateFilter : IAuthorizationFilter
	{
		private IRepository<User> userRepository;
		private IFormsAuthentication formsAuth;

		public AuthenticateFilter(IRepository<User> userRepository, IFormsAuthentication formsAuth)
		{
			this.userRepository = userRepository;
			this.formsAuth = formsAuth;
		}

		public void OnAuthorization(AuthorizationContext filterContext)
		{
			var context = filterContext.HttpContext;

			if(context.User != null && context.User.Identity.IsAuthenticated)
			{
				var email = context.User.Identity.Name;
				var user = userRepository.GetAll().WhereEmailIs(email);

				if (user == null) 
				{
					formsAuth.SignOut();
				}
				else 
				{
					AuthenticateAs(context, user);
					return;
				}
			}

			AuthenticateAs(context, User.Guest);
		}

		private void AuthenticateAs(HttpContextBase context, User user)
		{
			Thread.CurrentPrincipal = context.User = user;				
		}
	}
}