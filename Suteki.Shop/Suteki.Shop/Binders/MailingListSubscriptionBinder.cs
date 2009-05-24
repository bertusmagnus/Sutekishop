using System.Globalization;
using System.Web.Mvc;
using Suteki.Common.Binders;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;

namespace Suteki.Shop.Binders
{
	public class BindMailingListAttribute : DataBindAttribute
	{
		public BindMailingListAttribute() : base(typeof(MailingListSubscriptionBinder))
		{
			Fetch = false;
		}
	}

	public class MailingListSubscriptionBinder : DataBinder
	{
		public MailingListSubscriptionBinder(IValidatingBinder validatingBinder, IRepositoryResolver resolver) : base(validatingBinder, resolver)
		{
		}

		protected override object CreateInstance(System.Web.Mvc.ControllerContext controllerContext, System.Web.Mvc.ModelBindingContext bindingContext) 
		{
			var subscription = new MailingListSubscription
			{
				Contact = new Contact()
			};

			return subscription;
		}

		protected override void ValidateEntity(System.Web.Mvc.ModelBindingContext bindingContext, System.Web.Mvc.ControllerContext controllerContext, object entity) {

			var subscription = (MailingListSubscription)entity;

			base.ValidateEntity(bindingContext, controllerContext, entity);
			ValidateContact(bindingContext, controllerContext, subscription);


			base.ValidateEntity(bindingContext, controllerContext, subscription.Contact);

			var confirmEmail = controllerContext.HttpContext.Request.Form["emailconfirm"];

			if(subscription.Email != confirmEmail)
			{
				bindingContext.ModelState.AddModelError("emailconfirm", "Email and Confirm Email do not match");
				bindingContext.ModelState.SetModelValue("emailconfirm", new ValueProviderResult(confirmEmail??"", confirmEmail??"", CultureInfo.CurrentCulture));
			}

			
		}

		void ValidateContact(ModelBindingContext bindingContext, ControllerContext controllerContext, MailingListSubscription subscription)
		{
			try
			{
				validatingBinder.UpdateFrom(subscription.Contact, controllerContext.HttpContext.Request.Form,
				                            bindingContext.ModelState, bindingContext.ModelName + ".Contact");
			}
			catch(ValidationException)
			{
				//Error details are stored in modelstate - the exception does not need to be propagated. 
			}
		}
	}
}