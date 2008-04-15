using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Suteki.Shop.ViewData;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Controllers
{
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
            User defaultUser = new User { Email = "", Password = "", RoleId = 1 };
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

            BindingHelperExtensions.UpdateFrom(user, Request.Form);

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

        public void Edit(int id)
        {
            User user = userRepository.GetById(id);

            RenderView("Edit", new UserEditViewData { User = user, Roles = roleRepository.GetAll() });
        }
    }
}
