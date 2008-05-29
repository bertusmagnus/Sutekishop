using System;
using Suteki.Common.Extensions;
using Suteki.Common.Repositories;
using Suteki.Shop.Repositories;
using System.Web;

namespace Suteki.Shop.Services
{
    public class BaseControllerService : IBaseControllerService
    {
        public IRepository<Category> CategoryRepository { get; private set; }
        public IRepository<Content> ContentRepository { get; private set; }
        private string emailAddress;

        public string EmailAddress
        {
            get 
            {
                if (string.IsNullOrEmpty(emailAddress)) return string.Empty;
                return emailAddress; 
            }
            set { emailAddress = value; }
        }

        public virtual string SiteUrl 
        {
            get
            {

                Uri url = CurrentHttpContext.Request.Url;
                string relativePath = CurrentHttpContext.Request.ApplicationPath;
                string port = (url.Port == 80) ? "" : ":{0}".With(url.Port.ToString());

                return "{0}://{1}{2}{3}".With(url.Scheme, url.Host, port, relativePath);
            }
        }

        public virtual HttpContext CurrentHttpContext
        {
            get
            {
                if (HttpContext.Current == null)
                {
                    throw new ApplicationException("There is no current HttpContext");
                }
                return HttpContext.Current;
            }
        }

        public BaseControllerService(
            IRepository<Category> categoryRepository,
            IRepository<Content> contentRepository)
        {
            this.CategoryRepository = categoryRepository;
            this.ContentRepository = contentRepository;
        }
    }
}
