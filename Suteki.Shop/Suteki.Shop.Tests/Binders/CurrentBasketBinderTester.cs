using System;
using System.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Shop.Binders;
using Suteki.Shop.Services;

namespace Suteki.Shop.Tests.Binders
{
	[TestFixture]
	public class CurrentBasketBinderTester
	{
		User user;
		IUserService userService;
		CurrentBasketBinder binder;

		[SetUp]
		public void Setup()
		{
			user = new User();
			userService = MockRepository.GenerateStub<IUserService>();
			userService.Expect(x => x.CurrentUser).Return(user);
			binder = new CurrentBasketBinder(userService);
		}

		[Test]
		public void When_user_is_guest_user_should_be_promoted_to_customer()
		{
			user.RoleId = Role.GuestId;
			var customer = new User() { Email = "foo@bar" };
			userService.Expect(x => x.CreateNewCustomer()).Return(customer);

			binder.BindModel(new ControllerContext(), new ModelBindingContext());

			userService.AssertWasCalled(x => x.SetAuthenticationCookie(customer.Email));
			userService.AssertWasCalled(x => x.SetContextUserTo(customer));
		}

		[Test]
		public void Should_return_current_user_basket()
		{
			var result = binder.BindModel(new ControllerContext(), new ModelBindingContext());
			result.ShouldBe<Basket>();
		}
	}
}