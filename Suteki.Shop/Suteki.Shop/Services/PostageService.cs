using System;
using Suteki.Common.Repositories;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Services
{
    public class PostageService : IPostageService
    {
        private readonly IRepository<Postage> postageRepository;

        public PostageService(IRepository<Postage> postageRepository)
        {
            this.postageRepository = postageRepository;
        }

        public PostageResult CalculatePostageFor(Basket basket)
        {
            var postages = postageRepository.GetAll();

            return basket.CalculatePostage(postages);
        }

        public PostageResult CalculatePostageFor(Order order)
        {
            if (order.Basket == null)
            {
                throw new ApplicationException("Order has no basket");
            }

            return CalculatePostageFor(order.Basket);
        }
    }
}
