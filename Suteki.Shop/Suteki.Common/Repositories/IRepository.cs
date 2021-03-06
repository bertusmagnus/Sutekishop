﻿using System;
using System.Linq;
using System.Data.Linq;

namespace Suteki.Common.Repositories
{
    public interface IRepository<T> where T : class
    {
        T GetById(int id);
        IQueryable<T> GetAll();
        void InsertOnSubmit(T entity);
        void DeleteOnSubmit(T entity);
		[Obsolete("Units of Work should be managed externally to the Repository.")]
        void SubmitChanges();
    }

    public interface IRepository
    {
        object GetById(int id);
        IQueryable GetAll();
        void InsertOnSubmit(object entity);
        void DeleteOnSubmit(object entity);
		[Obsolete("Units of Work should be managed externally to the Repository.")]
        void SubmitChanges();
    }
}