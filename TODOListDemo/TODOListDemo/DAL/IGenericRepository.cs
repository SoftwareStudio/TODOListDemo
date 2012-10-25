using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TODOListDemo.DAL
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        IQueryable<TEntity> Query { get; }
        void Add(TEntity entity);
        void Edit(TEntity entity);
        void Delete(TEntity entity);
        void SaveChanges();
    }
}