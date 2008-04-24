using System;
using System.Linq;
using System.Data.Linq;
using Suteki.Shop.Repositories;

namespace Suteki.Shop.Services
{
    public class OrderableService<T> : IOrderServiceWithPosition<T>, IOrderableService<T> 
        where T : class, IOrderable
    {
        IRepository<T> repository;
        int postion;

        public OrderableService(IRepository<T> repository)
        {
            this.repository = repository;
        }

        public IOrderServiceWithPosition<T> MoveItemAtPosition(int postion)
        {
            OrderableService<T> orderService = new OrderableService<T>(repository);
            orderService.postion = postion;
            return orderService;
        }

        void IOrderServiceWithPosition<T>.UpOne()
        {
            Move.ItemAt(postion).In(OrderableItems).UpOne();
            repository.SubmitChanges();
        }

        void IOrderServiceWithPosition<T>.DownOne()
        {
            Move.ItemAt(postion).In(OrderableItems).DownOne();
            repository.SubmitChanges();
        }

        private IQueryable<IOrderable> OrderableItems
        {
            get
            {
                return repository.GetAll().Select(i => (IOrderable)i);
            }
        }

        public int NextPosition
        {
            get
            {
                return repository.GetAll().GetNextPosition();
            }
        }
    }

    public interface IOrderServiceWithPosition<T>
    {
        void UpOne();
        void DownOne();
    }
}
