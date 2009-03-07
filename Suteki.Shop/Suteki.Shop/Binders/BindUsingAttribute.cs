using System;
using System.Web.Mvc;
using Microsoft.Practices.ServiceLocation;
using Suteki.Common.Extensions;

namespace Suteki.Shop.Binders
{
	public class BindUsingAttribute : CustomModelBinderAttribute
	{
		private readonly Type binderType;

		public BindUsingAttribute(Type binderType)
		{
			if(!typeof(IModelBinder).IsAssignableFrom(binderType))
			{
				throw new InvalidOperationException("Type '{0}' does not implement IModelBinder.".With(binderType.Name));
			}

			this.binderType = binderType;
		}

		public override IModelBinder GetBinder()
		{
			var binder = (IModelBinder) ServiceLocator.Current.GetInstance(binderType);

			if(binder is IAcceptsAttribute)
			{
				((IAcceptsAttribute)binder).Accept(this);
			}

			return binder;
		}
	}
}