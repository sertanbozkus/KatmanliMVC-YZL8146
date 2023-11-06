using BilgeShop.Data.Entities;
using BilgeShop.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BilgeShop.Data.Repositories
{
    public interface IRepository<TEntity>
        where TEntity : class
    {
        void Add(TEntity entity);
        void Delete(TEntity entity);
        void Delete(int id);
        void Update(TEntity entity);
        TEntity GetById(int id);

        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null);

        // Bir sql sorgusunu (linq) parametre olarak göndermek istiyorsanız
        // Parametrenin tipi -> Expression<Func<TEntity, bool>> 

        // = null diyerek bu metodun parametre alarak veya almayarak çalışabileceğini gösteriyorum.

        // Parametre gönderilirse, o filtrelemeyle bütün yapılar.

        // Parametre gönderilmezse, filtrelemesiz bütün yapılar.

        TEntity Get(Expression<Func<TEntity, bool>> predicate);

  
  
    }
}


    