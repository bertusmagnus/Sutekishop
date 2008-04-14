using System;

namespace Suteki.Shop
{
    public partial class Role
    {
        public override string ToString()
        {
            return Name;
        }

        public const int AdministratorId = 1;
        public const int OrderProcessorId = 2;
        public const int CustomerId = 3;
        public const int GuestId = 4;

        // allowed roles. These must match the data in table Role
        public static Role Administrator { get { return new Role() { RoleId = AdministratorId, Name = "Administrator" }; } }
        public static Role OrderProcessor { get { return new Role() { RoleId = OrderProcessorId, Name = "Order Processor" }; } }
        public static Role Customer { get { return new Role() { RoleId = CustomerId, Name = "Customer" }; } }
        public static Role Guest { get { return new Role() { RoleId = GuestId, Name = "Guest" }; } }

        public bool IsAdministrator { get { return Name == Administrator.Name; } }
        public bool IsOrderProcessor { get { return Name == OrderProcessor.Name; } }
        public bool IsCustomer { get { return Name == Customer.Name; } }
        public bool IsGuest { get { return Name == Guest.Name; } }
    }
}
