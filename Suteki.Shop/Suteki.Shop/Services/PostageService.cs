using System;
using Suteki.Common.Repositories;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Services
{
    public class PostageService : IPostageService
    {
        private readonly IRepository<PostZone> postZoneRepository;
        private readonly IRepository<Postage> postageRepository;

        public PostageService(IRepository<PostZone> postZoneRepository, IRepository<Postage> postageRepository)
        {
            this.postZoneRepository = postZoneRepository;
            this.postageRepository = postageRepository;
        }

        public PostageService(IRepository<PostZone> postZoneRepository)
        {
            this.postZoneRepository = postZoneRepository;
        }

        public PostageResult CalculatePostageFor(Basket basket)
        {
            var postages = postageRepository.GetAll();
            var postZone = postZoneRepository.GetDefaultPostZone();

            return basket.CalculatePostage(postages, postZone);
        }

        public PostageResult CalculatePostageFor(Order order)
        {
            if (order.Basket == null)
            {
                throw new ApplicationException("Order has no basket");
            }

            var postages = postageRepository.GetAll();

            Contact contact = order.PostalContact;
            PostZone postZone;
            if (contact != null && contact.Country != null && contact.Country.PostZone != null)
            {
                postZone = contact.Country.PostZone;
            }
            else
            {
                postZone = postZoneRepository.GetDefaultPostZone();
            }

            return order.Basket.CalculatePostage(postages, postZone);
        }
    }
}
