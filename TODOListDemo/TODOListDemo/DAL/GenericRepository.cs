using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TODOListDemo.DAL
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private EFDbContext context = new EFDbContext();

        public IQueryable<TEntity> Query
        {
            get { return context.Set<TEntity>().AsQueryable(); }
        }

        public void Add(TEntity entity)
        {
            context.Set<TEntity>().Add(entity);           
        }

        public void Edit(TEntity entity)
        {
            context.Entry<TEntity>(entity).State = System.Data.EntityState.Modified;            
        }

        public void Delete(TEntity entity)
        {
            context.Set<TEntity>().Remove(entity);            
        }

        public void SaveChanges()
        {
            context.SaveChanges();
        }
    }
}