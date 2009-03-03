using System;
using System.Security.Principal;
using Suteki.Common.Validation;
using Suteki.Shop.Repositories;
using System.Web.Security;

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

    	public static User DefaultUser
    	{
			get { return new User { Email = "", Password = "", RoleId = 1, IsEnabled = true }; }
    	}

        public string PublicIdentity
        {
            get
            {
                if (CanLogin) return Email;
                return "Guest";
            }
        }

        public bool CanLogin { get { return IsAdministrator || IsOrderProcessor; } }

        public Basket CurrentBasket
        {
            get
            {
                if (Baskets.Count == 0)
                {
                    return CreateNewBasket();
                }
                return Baskets.CurrentBasket();
            }
        }

        public Basket CreateNewBasket()
        {
            // HACK, we're relying on the static data for the default country to be 1 (UK)
            return new Basket
                       {
                           User = this,
                           OrderDate = DateTime.Now,
                           CountryId = 1
                       };
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

        public bool IsAdministrator { get { return RoleId == Role.AdministratorId; } }
        public bool IsOrderProcessor { get { return RoleId == Role.OrderProcessorId; } }
        public bool IsCustomer { get { return RoleId == Role.CustomerId; } }
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
