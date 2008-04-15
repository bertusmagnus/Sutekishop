using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;
using System.Web.Security;
using System.Security.Permissions;
using Suteki.Shop.Validation;

namespace Suteki.Shop.Controllers
{
    [PrincipalPermission(SecurityAction.Demand, Role = "Administrator")]
    public class UserController : ControllerBase
    {
        IRepository<User> userRepository;
        IRepository<Role> roleRepository;

        public UserController(IRepository<User> userRepository, IRepository<Role> roleRepository)
        {
            this.userRepository = userRepository;
            this.roleRepository = roleRepository;
        }

        public void Index()
        {
            var users = userRepository.GetAll();
            RenderView("Index", new UserViewData { Users = users });
        }

        public void New()
        {
            User defaultUser = new User { Email = "", Password = "", RoleId = 1, IsEnabled = true };
            RenderView("Edit", new UserEditViewData { User = defaultUser, Roles = roleRepository.GetAll() });
        }

        public void Update(int userid)
        {
            User user = null;

            if (userid == 0)
            {
                user = new User();
            }
            else
            {
                user = userRepository.GetById(userid);
            }

            UpdateFromForm(user);

            try
            {
                user.Validate();
            }
            catch (ValidationException validationException)
            {
                RenderView("Edit", new UserEditViewData
                {
                    User = user,
                    Roles = roleRepository.GetAll(),
                    ErrorMessage = validationException.Message
                });
                return;
            }

            if (userid == 0)
            {
                userRepository.InsertOnSubmit(user);
            }

            userRepository.SubmitChanges();

            RenderView("Edit", new UserEditViewData 
            { 
                User = user, 
                Roles = roleRepository.GetAll(),
                Message = "Changes have been saved"
            });
        }

        private void UpdateFromForm(User user)
        {
            user.Email = this.ReadFromRequest("email");
            user.RoleId = int.Parse(this.ReadFromRequest("roleid"));
            user.IsEnabled = (this.ReadFromRequest("isenabled") == "True");

            string password = this.ReadFromRequest("password");
            
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

        public void Edit(int id)
        {
            User user = userRepository.GetById(id);

            RenderView("Edit", new UserEditViewData { User = user, Roles = roleRepository.GetAll() });
        }
    }
}
