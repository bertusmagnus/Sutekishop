namespace Suteki.Shop.Services
{
    public interface IPostageService
    {
        PostageResult CalculatePostageFor(Basket basket);
        PostageResult CalculatePostageFor(Order order);
    }
}
