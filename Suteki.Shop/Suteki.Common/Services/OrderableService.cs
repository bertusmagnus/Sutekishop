using System;
using System.Linq;
using Suteki.Common;
using Suteki.Common.Repositories;
using System.Linq.Expressions;
using Suteki.Common.Services;

namespace Suteki.Common.Services
{
    public class OrderableService<T> : 
        IOrderServiceWithPosition<T>, 
        IOrderServiceWithConstrainedPosition<T>, 
        IOrderableService<T> 
        where T : class, IOrderable
    {
        IRepository<T> repository;
        int postion;
        Expression<Func<T, bool>> predicate;

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
            Move<T>.ItemAt(postion).In(repository.GetAll()).UpOne();
        }

        void IOrderServiceWithPosition<T>.DownOne()
        {
            Move<T>.ItemAt(postion).In(repository.GetAll()).DownOne();
        }

        public int NextPosition
        {
            get
            {
                return repository.GetAll().GetNextPosition();
            }
        }

        IOrderServiceWithConstrainedPosition<T> IOrderServiceWithPosition<T>.ConstrainedBy(
            Expression<Func<T, bool>> predicate)
        {
            OrderableService<T> orderService = new OrderableService<T>(repository);
            orderService.postion = postion;
            orderService.predicate = predicate;
            return orderService;
        }

        void IOrderServiceWithConstrainedPosition<T>.UpOne()
        {
            Move<T>.ItemAt(postion).In(repository.GetAll().Where(predicate)).UpOne();
        }

        void IOrderServiceWithConstrainedPosition<T>.DownOne()
        {
            Move<T>.ItemAt(postion).In(repository.GetAll().Where(predicate)).DownOne();
        }
    }

    public interface IOrderServiceWithPosition<T>
    {
        void UpOne();
        void DownOne();
        IOrderServiceWithConstrainedPosition<T> ConstrainedBy(Expression<Func<T, bool>> predicate);
    }

    public interface IOrderServiceWithConstrainedPosition<T>
    {
        void UpOne();
        void DownOne();
    }
}