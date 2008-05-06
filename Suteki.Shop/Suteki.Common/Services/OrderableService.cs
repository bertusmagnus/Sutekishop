using System;
using System.Linq;
using System.Data.Linq;
using Suteki.Common.Repositories;
using System.Linq.Expressions;

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
            repository.SubmitChanges();
        }

        void IOrderServiceWithPosition<T>.DownOne()
        {
            Move<T>.ItemAt(postion).In(repository.GetAll()).DownOne();
            repository.SubmitChanges();
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
            repository.SubmitChanges();
        }

        void IOrderServiceWithConstrainedPosition<T>.DownOne()
        {
            Move<T>.ItemAt(postion).In(repository.GetAll().Where(predicate)).DownOne();
            repository.SubmitChanges();
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
