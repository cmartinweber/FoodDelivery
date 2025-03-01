using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Interfaces
{
    public interface IGenericRepository<T> where T : class
    {
        //Get a single object by it's key id
        T GetByID(int id);

        //used to get (SELECT/WHERE)
        //A Func<T, bool> represents a function that takes an object of type T and returns a bool.
        //It's commonl referred to as a "predicate",
        //It is used to verify a condition on an object.
        //Expression<Func<T>> is a description of a function as an expression tree.
        //It can ve compiled at run time that generates a Func<T>
        //It can also be translated to other languages e.g. SQL in LINQ to SQL.
        //NoTracking is ReadOnly Results, and INcludes is Join of other objects
        T Get(Expression<Func<T, bool>> predicate, bool asNoTracking = false, string includes = null);

        //same as above by Asynchronous action
        Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false, string includes = null);

        //Returns an Enumerable list of results to iterate through
        IEnumerable<T> List();

        //Returns an Enumerable list of results to iterate through. Expression is the query, optional Order By. Optional includes (join) other objects
        IEnumerable<T> List(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> orderBy = null, string includes = null);

        //same as above by Asynchronous action
        Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> orderBy = null, string includes = null);

        //Add (Insert) a new record instance
        void Add(T entity);

        //Delete (Remove) a single record instance
        void Delete(T entity);

        //Delete (Remove) multiple record instances
        void Delete(IEnumerable<T> entities);

        //UPdate all changes in an object
        void Update(T entity);
    }
}
