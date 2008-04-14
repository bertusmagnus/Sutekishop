using System;
using System.Linq;

namespace Suteki.Shop.Repositories
{
    public static class UserRepositoryExtensions
    {
        public static User WhereEmailIs(this IQueryable<User> users, string email)
        {
            return users.Where(user => user.Email == email).SingleOrDefault();
        }
    }
}
