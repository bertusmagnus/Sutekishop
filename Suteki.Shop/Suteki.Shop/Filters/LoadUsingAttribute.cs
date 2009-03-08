using System;
using System.Data.Linq;
using System.Linq;
using System.Web.Mvc;
using Suteki.Common.Binders;
using Suteki.Common.Filters;
using Suteki.Common.Repositories;
using Suteki.Shop.Models;

namespace Suteki.Shop.Filters
{
	//Note: The LoadUsingFilter MUST execute before all other filters otherwise you will get runtime errors.
	//The LoadOptions have to be set *before* anything tries to use the DataContext. 
	public class LoadUsingAttribute : FilterUsingAttribute
	{
		private readonly Type[] types;

		public LoadUsingAttribute(params Type[] types) : base(typeof(LoadUsingFilter))
		{
			this.types = types;
			Order = -1;
		}

		public Type[] Types
		{
			get { return types; }
		}
	}

	public class LoadUsingFilter : IAuthorizationFilter, IAcceptsAttribute
	{
		private ILoadOptions[] loadOptions = new ILoadOptions[0];
		private readonly IDataContextProvider contextProvider;

		public LoadUsingFilter(IDataContextProvider contextProvider)
		{
			this.contextProvider = contextProvider;
		}

		public void OnAuthorization(AuthorizationContext filterContext)
		{
			var dataLoadOptions = new DataLoadOptions();
			
			foreach(var loadOption in loadOptions)
			{
				loadOption.Build(dataLoadOptions);
			}

			contextProvider.DataContext.LoadOptions = dataLoadOptions;
		}

		public ILoadOptions[] LoadOptions
		{
			get { return loadOptions; }
		}

		public void Accept(Attribute attribute)
		{
			var options = ((LoadUsingAttribute)attribute);

			loadOptions = options.Types
				.Where(type => typeof(ILoadOptions).IsAssignableFrom(type))
				.Select(type => (ILoadOptions)Activator.CreateInstance(type))
				.ToArray();
		}
	}
}