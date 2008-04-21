using System;
using System.Security.Principal;
using Suteki.Shop.Validation;
using Suteki.Shop.Repositories;

namespace Suteki.Shop
{
    public partial class User : IPrincipal
    {
        public static User Guest
        {
            get
            {
                return new User() { Email = "Guest@guest.com", Role = Role.Guest };
            }
        }

        public Order CurrentOrder
        {
            get
            {
                if (Orders.Count == 0)
                {
                    return new Order
                    {
                        User = this,
                        OrderDate = DateTime.Now
                    };
                }
                return Orders.CurrentOrder();
            }
        }

        public IIdentity Identity
        {
            get
            {
                bool isAuthenticated = !(Role.Name == Role.Guest.Name);
                return new Identity(isAuthenticated, this.Email);
            }
        }

        public bool IsInRole(string role)
        {
            return this.Role.Name == role;
        }

        public void Validate()
        {
            Validator validator = new Validator
            {
                () => Email.Label("Email").IsRequired(),
                () => Password.Label("Password").IsRequired(),
            };

            validator.Validate();
        }

        public bool IsAdministrator { get { return UserId == Role.AdministratorId; } }
        public bool IsOrderProcessor { get { return UserId == Role.OrderProcessorId; } }
    }

    /// <summary>
    /// Simple implementation of IIdentity
    /// </summary>
    public class Identity : IIdentity
    {
        bool isAuthenticated;
        string name;

        public Identity(bool isAuthenticated, string name)
        {
            this.isAuthenticated = isAuthenticated;
            this.name = name;
        }

        public string AuthenticationType
        {
            get { return "Forms"; }
        }

        public bool IsAuthenticated
        {
            get { return isAuthenticated; }
        }

        public string Name
        {
            get { return name; }
        }
    }
}
