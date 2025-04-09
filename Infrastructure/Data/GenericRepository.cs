using ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _dbContext;

        public GenericRepository(ApplicationDbContext context)
        {
            _dbContext = context;
        }

        public void Add(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            _dbContext.SaveChanges();
        }

        public void Delete(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
            _dbContext.SaveChanges();
        }

        public virtual T Get(Expression<Func<T, bool>> predicate, bool asNoTracking = false, string includes = null)
        {
            if (includes == null) //there are no tables join. Single Object
            {
                if (asNoTracking)
                { //read only copy for display purposes
                    return _dbContext.Set<T>().AsNoTracking().Where(predicate).FirstOrDefault();
                }
                else
                { //it needs to be tracked
                    return _dbContext.Set<T>().Where(predicate).FirstOrDefault();
                }
            }
            else //this has includes (other objects or tables)
            {
                IQueryable<T> queryable = _dbContext.Set<T>();
                foreach (var includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty.Trim());
                }
                if (asNoTracking)
                { //read only copy for display purposes
                    return queryable.AsNoTracking().Where(predicate).FirstOrDefault();
                }
                else
                { //it needs to be tracked
                    return queryable.Where(predicate).FirstOrDefault();
                }
            }
        }

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool asNoTracking = false, string includes = null)
        {
            if (includes == null) //there are no tables join. Single Object
            {
                if (asNoTracking)
                { //read only copy for display purposes
                    return await _dbContext.Set<T>().AsNoTracking().Where(predicate).FirstOrDefaultAsync();
                }
                else
                { //it needs to be tracked
                    return await _dbContext.Set<T>().Where(predicate).FirstOrDefaultAsync();
                }
            }
            else //this has includes (other objects or tables)
            {
                IQueryable<T> queryable = _dbContext.Set<T>();
                foreach (var includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty.Trim());
                }
                if (asNoTracking)
                { //read only copy for display purposes
                    return await queryable.AsNoTracking().Where(predicate).FirstOrDefaultAsync();
                }
                else
                { //it needs to be tracked
                    return await queryable.Where(predicate).FirstOrDefaultAsync();
                }
            }
        }

        public virtual T GetByID(int id)
        {
            return _dbContext.Set<T>().Find(id);
        }

        public IEnumerable<T> List()
        {
            return _dbContext.Set<T>().ToList().AsEnumerable();
        }

        public virtual IEnumerable<T> List(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> orderBy = null, string includes = null)
        {
            IQueryable<T> queryable = _dbContext.Set<T>();
            if (predicate != null && includes == null)
            {//does have a where, but does not include others
                return _dbContext.Set<T>().Where(predicate).AsEnumerable();
            }
            else if (includes != null)
            {//are included joins
                foreach (var includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty.Trim());
                }
            }
            if (predicate == null)
            {
                if (orderBy == null)
                {
                    return queryable.AsEnumerable();
                }
                else
                {
                    return queryable.OrderBy(orderBy).ToList().AsEnumerable();
                }
            }
            else
            {
                if (orderBy == null)
                {
                    return queryable.Where(predicate).ToList().AsEnumerable();
                }
                else
                {
                    return queryable.Where(predicate).OrderBy(orderBy).ToList().AsEnumerable();
                }
            }
        }

        public virtual async Task<IEnumerable<T>> ListAsync(Expression<Func<T, bool>> predicate, Expression<Func<T, int>> orderBy = null, string includes = null)
        {
            IQueryable<T> queryable = _dbContext.Set<T>();
            if (predicate != null && includes == null)
            {//does have a where, but does not include others
                return await _dbContext.Set<T>().Where(predicate).ToListAsync();
            }
            else if (includes != null)
            {//are included joins
                foreach (var includeProperty in includes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    queryable = queryable.Include(includeProperty.Trim());
                }
            }
            if (predicate == null)
            {
                if (orderBy == null)
                {
                    return await queryable.ToListAsync();
                }
                else
                {
                    return await queryable.OrderBy(orderBy).ToListAsync();
                }
            }
            else
            {
                if (orderBy == null)
                {
                    return await queryable.Where(predicate).ToListAsync();
                }
                else
                {
                    return await queryable.Where(predicate).OrderBy(orderBy).ToListAsync();
                }
            }
        }

        public void Update(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            _dbContext.SaveChanges();
        }
    }
}
