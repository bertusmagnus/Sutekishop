using System;
using System.Web.Mvc;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;
using Suteki.Common.Extensions;

namespace Suteki.Shop.Binders
{
	public class DataBindAttribute : BindUsingAttribute
	{
		public DataBindAttribute() : base(typeof(DataBinder))
		{
		}
	}

	public class DataBinder : IModelBinder
	{
		private readonly IValidatingBinder validatingBinder;
		private readonly IRepositoryResolver resolver;

		public DataBinder(IValidatingBinder validatingBinder, IRepositoryResolver resolver)
		{
			this.validatingBinder = validatingBinder;
			this.resolver = resolver;
		}

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			var primaryKey = bindingContext.ModelType.GetPrimaryKey();
			string name = bindingContext.ModelName + "." + primaryKey.Name;

			string rawKeyValue = controllerContext.HttpContext.Request.Form[name];

			if(string.IsNullOrEmpty(rawKeyValue))
			{
				throw new InvalidOperationException("Could not find a value named '{0}'".With(name));
			}

			int key = Convert.ToInt32(rawKeyValue);
			var repository = resolver.GetRepository(bindingContext.ModelType);
			var entity = repository.GetById(key);

			try
			{
				validatingBinder.UpdateFrom(entity, controllerContext.HttpContext.Request.Form, bindingContext.ModelState, bindingContext.ModelName);
			}
			catch(ValidationException ex) 
			{
				//Ignore validation exceptions - they are stored in ModelState. 
				//The controller can access the errors by inspecting the ModelState dictionary.
			}

			return entity;
		}

	}
}