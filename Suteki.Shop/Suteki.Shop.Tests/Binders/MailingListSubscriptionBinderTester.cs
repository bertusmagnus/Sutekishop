using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using NUnit.Framework;
using Rhino.Mocks;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;
using Suteki.Shop.Binders;

namespace Suteki.Shop.Tests.Binders
{
	[TestFixture]
	public class MailingListSubscriptionBinderTester
	{
		MailingListSubscriptionBinder binder;
		ModelBindingContext context;
		FakeValueProvider valueProvider;
		ControllerContext controllerContext;
		IValidatingBinder validatingBinder;

		[SetUp]
		public void Setup()
		{
			validatingBinder = MockRepository.GenerateStub<IValidatingBinder>();
			binder = new MailingListSubscriptionBinder(validatingBinder,
			                                           MockRepository.GenerateStub<IRepositoryResolver>());

			binder.Accept(new BindMailingListAttribute());

			valueProvider = new FakeValueProvider();
			context = new ModelBindingContext()
			{
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(null, typeof(MailingListSubscription)),
				ModelState =  new ModelStateDictionary(),
				ValueProvider = valueProvider,
				ModelName = "subscription",
			};

			controllerContext = new ControllerContext
			{
				HttpContext = MockRepository.GenerateStub<HttpContextBase>() 				
			};
			controllerContext.HttpContext.Expect(x => x.Request).Return(MockRepository.GenerateStub<HttpRequestBase>());
			controllerContext.HttpContext.Request.Expect(x => x.Form).Return(new NameValueCollection());
		}

		[Test]
		public void AddsErrorToModelState_WhenEmailsDoNotMatch()
		{
			valueProvider.AddValue("subscription.Email", "foo", "foo");
			controllerContext.HttpContext.Request.Form.Add("emailconfirm", "bar");

			var instance = binder.BindModel(controllerContext, context);
			context.ModelState["emailconfirm"].Errors.Single().ErrorMessage.ShouldEqual("Email and Confirm Email do not match");
		}

		[Test]
		public void InstantiatesContact()
		{
			var instance = (MailingListSubscription) binder.BindModel(controllerContext, context);
			instance.Contact.ShouldNotBeNull();
		}

		[Test]
		public void Validates_Contact()
		{
			var instance = (MailingListSubscription) binder.BindModel(controllerContext, context);
			validatingBinder.AssertWasCalled(x => x.UpdateFrom(instance.Contact, controllerContext.HttpContext.Request.Form, context.ModelState, "subscription.Contact"));
		}
	}
}