using System.Collections.Specialized;
using System.Web;
using Suteki.Common.Validation;

namespace Suteki.Common.Services
{
    /// <summary>
    /// Provides HttpContext IoC stylie
    /// </summary>
    public class HttpContextService : IHttpContextService
    {
        private readonly IValidatingBinder validatingBinder;

        public HttpContextService(IValidatingBinder validatingBinder)
        {
            this.validatingBinder = validatingBinder;
        }

        public HttpContextBase Context
        {
            get
            {
                return new HttpContextWrapper(HttpContext.Current);
            }
        }

        public HttpRequestBase Request
        {
            get
            {
                return Context.Request;
            }
        }

        public HttpResponseBase Response
        {
            get
            {
                return Context.Response;
            }
        }

        public NameValueCollection FormOrQuerystring
        {
            get
            {
                if(Request.RequestType == "POST")
                {
                    return Request.Form;
                }
                return Request.QueryString;
            }
        }

        public void BindToForm(object target)
        {
            validatingBinder.UpdateFrom(target, FormOrQuerystring);
        }
    }
}