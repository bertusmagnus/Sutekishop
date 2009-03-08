using System;
using System.Web.Mvc;
using Suteki.Common.Binders;
using Suteki.Shop.Services;

namespace Suteki.Shop.Binders
{
	public class CurrentBasketAttribute : BindUsingAttribute
	{
		public CurrentBasketAttribute() : base(typeof(CurrentBasketBinder))
		{
		}
	}

	public class CurrentBasketBinder : IModelBinder
	{
		private IUserService userService;

		public CurrentBasketBinder(IUserService userService)
		{
			this.userService = userService;
		}

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var user = userService.CurrentUser;

			if(UserIsGuest(user))
			{
				user = PromoteGuestToCustomer();
			}

			return user.CurrentBasket;
		}

		private User PromoteGuestToCustomer()
		{
			var user = userService.CreateNewCustomer();
			userService.SetAuthenticationCookie(user.Email);
			userService.SetContextUserTo(user);
			return user;
		}

		private bool UserIsGuest(User user)
		{
			return user.RoleId == Role.GuestId;
		}
	}
}