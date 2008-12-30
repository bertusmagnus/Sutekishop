using System;
using System.Collections.Specialized;
using System.Web.Mvc;

namespace Suteki.Common.Validation
{
    public class BindingContext
    {
        public object Target { get; private set; }
        public NameValueCollection Values { get; private set; }
        public string ObjectPrefix { get; private set; }
        public ModelStateDictionary ModelStateDictionary { get; private set; }
        public string AttemptedValue { get; private set; }

        public BindingContext(
            object target, 
            NameValueCollection values, 
            string objectPrefix, 
            ModelStateDictionary modelStateDictionary)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }
            if (modelStateDictionary == null)
            {
                throw new ArgumentNullException("modelStateDictionary");
            }

            Target = target;
            Values = values;
            ObjectPrefix = objectPrefix;
            ModelStateDictionary = modelStateDictionary;
        }

        public virtual void AddModelError(string key, string attemptedValue, Exception exception)
        {
            ModelStateDictionary.AddModelError(key, exception);
        }

        public virtual void AddModelError(string key, string attemptedValue, string errorMessage)
        {
            ModelStateDictionary.AddModelError(key, errorMessage);
        }

        public virtual string GetValue(string propertyName)
        {
            var name = propertyName;
            var typeName = Target.GetType().Name;

            if (!string.IsNullOrEmpty(ObjectPrefix))
            {
                name = ObjectPrefix + "." + propertyName;
            }
            if (Values[name] == null)
            {
                name = typeName + "." + propertyName;
            }
            if (Values[name] == null)
            {
                name = typeName + "_" + propertyName;
            }

            AttemptedValue = Values[name];
            return AttemptedValue;
        }
    }
}