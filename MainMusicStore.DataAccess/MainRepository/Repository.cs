using MainMusicStore.DataAccess.IMainRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using MainMusicStore.DataAccess.Data;
using Microsoft.EntityFrameworkCore;

namespace MainMusicStore.DataAccess.MainRepository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly MainMusicStoreDbContext _mainMusicStoreDbContext;

        internal DbSet<T> DbSet;

        public Repository(MainMusicStoreDbContext mainMusicStoreDbContext)
        {
            _mainMusicStoreDbContext = mainMusicStoreDbContext;
            DbSet = _mainMusicStoreDbContext.Set<T>();
        }

        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public T Get(int id)
        {
           return DbSet.Find(id);
        }

        public IEnumerable<T> GetAll(Expression<Func<T, bool>> filterExpression = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderByFunc = null, string includeProperties = null)
        {
            IQueryable<T> queryableTypeDbSet = DbSet;

            if (filterExpression != null)
            {
                queryableTypeDbSet = queryableTypeDbSet.Where(filterExpression);
            }

            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryableTypeDbSet = queryableTypeDbSet.Include(item);
                }
            }

            if (orderByFunc != null)
            {
                return orderByFunc(queryableTypeDbSet).ToList();
            }
            return queryableTypeDbSet.ToList();
        }

        public T GetFirstOrDefault(Expression<Func<T, bool>> filterExpression = null, string includeProperties = null)
        {
            IQueryable<T> queryableTypeDbSet = DbSet;

            if (filterExpression != null)
            {
                queryableTypeDbSet = queryableTypeDbSet.Where(filterExpression);
            }

            if (includeProperties != null)
            {
                foreach (var item in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryableTypeDbSet = queryableTypeDbSet.Include(item);
                }
            }
            return queryableTypeDbSet.FirstOrDefault();
        }

        public void Remove(int id)
        {
            T entity = DbSet.Find(id);
            Remove(entity);
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        /// <summary>
        /// Remove Range Multiple Entity
        /// </summary>
        /// <param name="entity"></param>
        public void RemoveRange(IEnumerable<T> entity)
        {
            DbSet.RemoveRange(entity);
        }
    }
}
