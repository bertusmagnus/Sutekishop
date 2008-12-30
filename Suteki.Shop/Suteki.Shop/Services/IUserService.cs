namespace Suteki.Shop.Services
{
    public interface IUserService
    {
        User CreateNewCustomer();
        User CurrentUser { get; }
        void SetAuthenticationCookie(string email);
        void SetContextUserTo(User user);
        void RemoveAuthenticationCookie();
    }
}
