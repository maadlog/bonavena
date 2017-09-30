using Bonavena.Dao;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bonavena.Repository
{
    public interface IRepository<T> where T : class, new()
    {
        int Save(T entity);

        void Delete(int id);

        void Delete(T entity);

        List<T> Find(T filterEntity);

        List<T> GetAll();

        String CallFunction(string fnName, T entity);
    }

    public class Repository<T> : IRepository<T> where T : class, new()
    {
        public int Save(T entity)
        {
            return GenericDAO<T>.Save(entity);
        }

        public void Delete(int id)
        {
            GenericDAO<T>.Delete(id);
        }

        public void Delete(T entity)
        {
            GenericDAO<T>.Delete(entity);
        }

        public List<T> Find(T filterEntity)
        {
            return GenericDAO<T>.Find(filterEntity);
        }

        public List<T> GetAll()
        {
            return GenericDAO<T>.GetAll();
        }

        public String CallFunction(string fnName, T entity)
        {
            return GenericDAO<T>.CallFunction(fnName, entity);
        }
    }
}
