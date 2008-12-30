using System.Collections.Specialized;
using System.Web.Mvc;

namespace Suteki.Common.Validation
{
    public interface IValidatingBinder : IModelBinder 
    {
        void UpdateFrom(object target, NameValueCollection values);
        void UpdateFrom(object target, NameValueCollection values, string objectPrefix);
        void UpdateFrom(object target, NameValueCollection values, ModelStateDictionary modelStateDictionary);
        void UpdateFrom(object target, NameValueCollection values, ModelStateDictionary modelStateDictionary, string objectPrefix);
    }
}