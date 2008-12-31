using System.Collections.Specialized;
using System.Web.Mvc;
using Suteki.Common.Repositories;
using Suteki.Common.Validation;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using System.Web.Security;
using System.Security.Permissions;

namespace Suteki.Shop.Controllers
{
    [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
    public class UserController : ControllerBase
    {
        readonly IRepository<User> userRepository;
        readonly IRepository<Role> roleRepository;

        public UserController(IRepository<User> userRepository, IRepository<Role> roleRepository)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
        }

        public ActionResult Index()
        {
            var users = userRepository.GetAll().Editable();
            return View("Index", ShopView.Data.WithUsers(users));
        }

        public ActionResult New()
        {
            User defaultUser = new User { Email = "", Password = "", RoleId = 1, IsEnabled = true };
            return View("Edit", EditViewData.WithUser(defaultUser));
        }

        public ActionResult Edit(int id)
        {
            User user = userRepository.GetById(id);
            return View("Edit", EditViewData.WithUser(user));
        }

        public ActionResult Update(int userid, FormCollection form)
        {
            var user = userid == 0 ? 
                new User() : 
                userRepository.GetById(userid);

            UpdateFromForm(user, form);

            try
            {
                user.Validate();
            }
            catch (ValidationException validationException)
            {
                return View("Edit", EditViewData.WithUser(user).WithErrorMessage(validationException.Message));
            }

            if (userid == 0)
            {
                userRepository.InsertOnSubmit(user);
            }

            userRepository.SubmitChanges();

            return View("Edit", EditViewData.WithUser(user).WithMessage("Changes have been saved")); 
        }

        /// <summary>
        /// Have to provide custom update functionality because of the quirks of having a password
        /// </summary>
        /// <param name="user"></param>
        /// <param name="form"></param>
        private void UpdateFromForm(User user, NameValueCollection form)
        {
            user.Email = form["email"];
            user.RoleId = int.Parse(form["roleid"]);
            user.IsEnabled = (form["isenabled"] == "true,false");

            string password = form["password"];
            
            if (!string.IsNullOrEmpty(password))
            {
                user.Password = password;
                EncryptPassword(user);
            }
        }

        [NonAction]
        public virtual void EncryptPassword(User user)
        {
            user.Password = FormsAuthentication.HashPasswordForStoringInConfigFile(user.Password, "SHA1");
        }

        public ShopViewData EditViewData
        {
            get
            {
                return ShopView.Data.WithRoles(roleRepository.GetAll());
            }
        }
    }
}
