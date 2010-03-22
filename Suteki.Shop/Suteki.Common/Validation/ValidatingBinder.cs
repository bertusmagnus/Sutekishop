using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.Reflection;
using System.Web.Mvc;
using Suteki.Common.Extensions;

namespace Suteki.Common.Validation
{
    /// <summary>
    /// An implementation of IValidatingBinder that allows the user to add a pipeline of property binders.
    /// Each property binder in turn has a chance to bind a given property. See tests for more details.
    /// 
    /// Example set up:
    /// 
    /// validatingBinder = new ValidatingBinder(
    ///                 new SimplePropertyBinder(),
    ///                 new BooleanPropertyBinder());
    /// </summary>
    public class ValidatingBinder : IValidatingBinder
    {
        private readonly List<IBindProperties> propertyBinders;

        public ValidatingBinder() : this(new IBindProperties[0])
        {
        }

        public ValidatingBinder(params IBindProperties[] propertyBinders)
        {
            this.propertyBinders = new List<IBindProperties>(propertyBinders);
        }

        public List<IBindProperties> PropertyBinders
        {
            get { return propertyBinders; }
        }

        public virtual void UpdateFrom(object target, NameValueCollection values)
        {
            UpdateFrom(target, values, new ModelStateDictionary(), null);
        }

        public virtual void UpdateFrom(object target, NameValueCollection values, ModelStateDictionary modelStateDictionary)
        {
            UpdateFrom(target, values, modelStateDictionary, null);
        }

        public virtual void UpdateFrom(object target, NameValueCollection values, string objectPrefix)
        {
            UpdateFrom(target, values, new ModelStateDictionary(), objectPrefix);
        }

        public virtual void UpdateFrom(
            object target, 
            NameValueCollection values, 
            ModelStateDictionary modelStateDictionary, 
            string objectPrefix)
        {
            UpdateFrom(new BindingContext(target, values, objectPrefix, modelStateDictionary));
        }

        public virtual void UpdateFrom(BindingContext bindingContext)
        {
            foreach (var property in bindingContext.Target.GetType().GetProperties())
            {
                try
                {
                    foreach (var binder in propertyBinders)
                    {
                        binder.Bind(property, bindingContext);
                    }
                }
                catch (Exception exception)
                {
                    if (exception.InnerException is FormatException ||
                        exception.InnerException is IndexOutOfRangeException)
                    {
						string key = BuildKeyForModelState(property, bindingContext.ObjectPrefix);
                        bindingContext.AddModelError(key, bindingContext.AttemptedValue, "Invalid value for {0}".With(property.Name));
						bindingContext.ModelStateDictionary.SetModelValue(key, new ValueProviderResult(bindingContext.AttemptedValue, bindingContext.AttemptedValue, CultureInfo.CurrentCulture));
                    }
                    else if (exception is ValidationException)
                    {
						string key = BuildKeyForModelState(property, bindingContext.ObjectPrefix);
                        bindingContext.AddModelError(key, bindingContext.AttemptedValue, exception.Message);
						bindingContext.ModelStateDictionary.SetModelValue(key, new ValueProviderResult(bindingContext.AttemptedValue, bindingContext.AttemptedValue, CultureInfo.CurrentCulture));
                    }
                    else if (exception.InnerException is ValidationException)
                    {
						string key = BuildKeyForModelState(property, bindingContext.ObjectPrefix);
                        bindingContext.AddModelError(key, bindingContext.AttemptedValue, exception.InnerException.Message);
						bindingContext.ModelStateDictionary.SetModelValue(key, new ValueProviderResult(bindingContext.AttemptedValue, bindingContext.AttemptedValue, CultureInfo.CurrentCulture));
                    }
                    else
                    {
                        throw;
                    }
                }

            }
            if (!bindingContext.ModelStateDictionary.IsValid)
            {
                throw new ValidationException("Bind Failed. See ModelStateDictionary for errors");
            }
        }

		private string BuildKeyForModelState(PropertyInfo property, string prefix)
		{
			if(string.IsNullOrEmpty(prefix))
			{
				return property.Name;
			}

			return prefix + "." + property.Name;
		}

        private static bool IsBasicType(Type type)
        {
            // TODO: find a better way to deal with Nullable<> types
            return (
                type.IsPrimitive ||
                type.IsEnum ||
                type == typeof(decimal) ||
                type == typeof(Guid) ||
                type == typeof(DateTime) ||
                type == typeof(string)) ||
                type == typeof(int?) ||
                type == typeof(bool?);
        }

        /// <summary>
        /// IModelBinder.BindModel
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="bindingContext"></param>
        /// <returns></returns>
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException("bindingContext");
            }

            if (IsBasicType(bindingContext.ModelType))
            {
                return new DefaultModelBinder().BindModel(controllerContext, bindingContext);
            }

            var instance = Activator.CreateInstance(bindingContext.ModelType);
            var request = controllerContext.HttpContext.Request;

            var form = request.RequestType == "POST" ? request.Form : request.QueryString;

            UpdateFrom(instance, form);

            return instance;
        }
    }
}