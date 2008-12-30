using System.Linq;
using Castle.Facilities.NHibernateIntegration;
using NHibernate;
using NHibernate.Linq;

namespace Suteki.Common.Repositories
{
    public class NHibernateRepository<T> : IRepository<T>, IRepository where T : class
    {
        private readonly ISessionManager sessionManager;

        public NHibernateRepository(ISessionManager sessionManager)
        {
            this.sessionManager = sessionManager;
        }

        private ISession Session
        {
            get
            {
                return sessionManager.OpenSession();
            }
        }

        public T GetById(int id)
        {
            return Session.Load<T>(id);
        }

        public IQueryable<T> GetAll()
        {
            return Session.Linq<T>();
        }

        public void InsertOnSubmit(T entity)
        {
            Session.Save(entity);
        }

        public void DeleteOnSubmit(T entity)
        {
            Session.Delete(entity);
        }

        public void SubmitChanges()
        {
            Session.Flush();
        }

        object IRepository.GetById(int id)
        {
            return GetById(id);
        }

        IQueryable IRepository.GetAll()
        {
            return GetAll();
        }

        void IRepository.InsertOnSubmit(object entity)
        {
            InsertOnSubmit((T)entity);
        }

        void IRepository.DeleteOnSubmit(object entity)
        {
            DeleteOnSubmit((T)entity);
        }
    }
}