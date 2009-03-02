using System;
using System.Web;
using System.Web.Security;
using Suteki.Common.Repositories;

namespace Suteki.Shop.Services
{
    public class UserService : IUserService
    {
        readonly IRepository<User> userRepository;
		readonly IFormsAuthentication formsAuth;

        public UserService(IRepository<User> userRepository, IFormsAuthentication formsAuth)
        {
        	this.userRepository = userRepository;
        	this.formsAuth = formsAuth;
        }

    	public User CreateNewCustomer()
        {
            var user = new User
            {
                Email = Guid.NewGuid().ToString(),
                Password = "",
                RoleId = Role.CustomerId
            };

            userRepository.InsertOnSubmit(user);
            userRepository.SubmitChanges();

            return user;
        }

        public virtual User CurrentUser
        {
            get
            {
                var user = HttpContext.Current.User as User;
                if (user == null) throw new ApplicationException("HttpContext.User is not a Suteki.Shop.User");
                return user;
            }
        }

        public virtual void SetAuthenticationCookie(string email)
        {
        	formsAuth.SetAuthCookie(email, true);
        }

        public virtual void SetContextUserTo(User user)
        {
            System.Threading.Thread.CurrentPrincipal = HttpContext.Current.User = user;
        }

        public virtual void RemoveAuthenticationCookie()
        {
			formsAuth.SignOut();
        }

    	public string HashPassword(string password)
    	{
    		return formsAuth.HashPasswordForStoringInConfigFile(password);
    	}
    }
}
