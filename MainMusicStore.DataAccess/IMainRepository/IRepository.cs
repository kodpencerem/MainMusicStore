using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MainMusicStore.DataAccess.IMainRepository
{
    public interface IRepository<T> where T: class
    {
        /// <summary>
        /// Bir id'ye verilerek kaydı çekme
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T Get(int id);

        /// <summary>
        /// Bir ya da birden fazla kaydı belli bir sıralamaya göre isteme
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <param name="orderByFunc"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        IEnumerable<T> GetAll(Expression<Func<T, bool>> filterExpression = null , Func<IQueryable<T>, IOrderedQueryable<T>> orderByFunc = null, string includeProperties=null);


        /// <summary>
        /// İlk bulunan kaydı getirme
        /// </summary>
        /// <param name="filterExpression"></param>
        /// <param name="includeProperties"></param>
        /// <returns></returns>
        T GetFirstOrDefault(Expression<Func<T, bool>> filterExpression = null ,  string includeProperties=null);

        void Add(T entity);


        /// <summary>
        /// İd'ye bağlı kayıt silme
        /// </summary>
        /// <param name="id"></param>
        void Remove(int id);

        /// <summary>
        /// Çoklu kayıt silme
        /// </summary>
        /// <param name="entity"></param>
        void Remove(T entity);

        /// <summary>
        /// Belli bir aralık içerisnde olan kayıtları silme
        /// </summary>
        /// <param name="entity"></param>
        void RemoveRange(IEnumerable<T> entity);
    }
}
