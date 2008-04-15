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

        public static bool ContainsUser(this IQueryable<User> users, string email, string password)
        {
            return users.Any(user => user.Email == email && user.Password == password);
        }
    }
}
