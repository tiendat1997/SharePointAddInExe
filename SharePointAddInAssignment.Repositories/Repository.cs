using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SharePointAddInAssignment.Repositories
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
        IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate);
        T GetById(object id);
        void Insert(T obj);
        void InsertRange(List<T> list);
        void Update(T obj);
        void Delete(T obj);
        void DeleteRange(Expression<Func<T, bool>> predicate);
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        protected DbContext context = null;
        protected DbSet<T> entity = null;

        public Repository(DbContext context)
        {
            this.context = context;
            this.entity = context.Set<T>();
        }

        public void Delete(T obj)
        {
            entity.Remove(obj);
        }

        public IEnumerable<T> GetAll()
        {
            return entity.ToList();
        }

        public T GetById(object id)
        {
            return entity.Find(id);
        }

        public void Insert(T obj)
        {
            entity.Add(obj);
        }

        public void Update(T obj)
        {
            context.Entry(obj).State = EntityState.Modified;
        }

        public IEnumerable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return entity.Where(predicate).ToList();
        }

        public void InsertRange(List<T> entities)
        {
            context.Set<T>().AddRange(entities);
        }

        public void DeleteRange(Expression<Func<T, bool>> predicate)
        {
            entity.RemoveRange(entity.Where(predicate));
        }
    }
}
