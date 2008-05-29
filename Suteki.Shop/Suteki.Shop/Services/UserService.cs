using System;
using Suteki.Common.Repositories;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Services
{
    public class UserService : IUserService
    {
        IRepository<User> userRepository;

        public UserService(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        public User CreateNewCustomer()
        {
            User user = new User
            {
                Email = Guid.NewGuid().ToString(),
                Password = "",
                RoleId = Role.CustomerId
            };

            userRepository.InsertOnSubmit(user);
            userRepository.SubmitChanges();

            return user;
        }
    }
}
