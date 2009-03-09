using System;
using System.Linq;
using Suteki.Common.Repositories;

namespace Suteki.Shop.Models
{
	//Repository implementation that delegates to the underlying Content repository. 
	public class MenuRepository : IRepository<Menu>, IRepository
	{
		private IRepository<Content> innerRepository;

		public MenuRepository(IRepository<Content> underlyingRepository)
		{
			this.innerRepository = underlyingRepository;
		}

		public Menu GetById(int id)
		{
			return innerRepository.GetById(id) as Menu;
		}

		public IQueryable<Menu> GetAll()
		{
			return innerRepository.GetAll().OfType<Menu>();
		}

		public void InsertOnSubmit(Menu entity)
		{
			innerRepository.InsertOnSubmit(entity);
		}

		public void DeleteOnSubmit(Menu entity)
		{
			innerRepository.DeleteOnSubmit(entity);
		}

		public void SubmitChanges()
		{
			innerRepository.SubmitChanges();
		}

		#region Non-generic
		object IRepository.GetById(int id) 
		{
			return GetById(id);
		}

		void IRepository.DeleteOnSubmit(object entity) 
		{
			innerRepository.DeleteOnSubmit((Content)entity);
		}

		void IRepository.InsertOnSubmit(object entity) 
		{
			innerRepository.InsertOnSubmit((Content)entity);
		}

		IQueryable IRepository.GetAll() 
		{
			return GetAll();
		}
		#endregion
	}
}