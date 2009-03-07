using System;
using System.Web.Mvc;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;
using Suteki.Common.Extensions;

namespace Suteki.Common.Binders
{
	public class DataBindAttribute : BindUsingAttribute
	{
		public DataBindAttribute() : base(typeof(DataBinder))
		{
			Fetch = true;
		}

		public bool Fetch { get; set; }
	}

	public class DataBinder : IModelBinder, IAcceptsAttribute
	{
		private readonly IValidatingBinder validatingBinder;
		private readonly IRepositoryResolver resolver;
		private DataBindAttribute declaringAttribute;
		
		public DataBinder(IValidatingBinder validatingBinder, IRepositoryResolver resolver)
		{
			this.validatingBinder = validatingBinder;
			this.resolver = resolver;
		}

		public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
		{
			object entity;

			if(declaringAttribute == null || declaringAttribute.Fetch)
			{
				entity = FetchEntity(bindingContext, controllerContext);
			}
			else 
			{
				entity = Activator.CreateInstance(bindingContext.ModelType);
			}
			

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

		private object FetchEntity(ModelBindingContext bindingContext, ControllerContext controllerContext)
		{
			object entity;
			var primaryKey = bindingContext.ModelType.GetPrimaryKey();
			string name = bindingContext.ModelName + "." + primaryKey.Name;

			string rawKeyValue = controllerContext.HttpContext.Request.Form[name];

			if (string.IsNullOrEmpty(rawKeyValue))
			{
				throw new InvalidOperationException("Could not find a value named '{0}'".With(name));
			}

			int key = Convert.ToInt32(rawKeyValue);
			var repository = resolver.GetRepository(bindingContext.ModelType);
			entity = repository.GetById(key);
			return entity;
		}

		public void Accept(Attribute attribute)
		{
			declaringAttribute = (DataBindAttribute) attribute;			
		}
	}
}