using Microsoft.EntityFrameworkCore;
using RedCrossChat.Contracts;
using RedCrossChat.Domain;
using System.Linq.Expressions;

namespace RedCrossChat.Repository
{
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected AppDBContext AppDbContext { get; set; }

        public RepositoryBase(AppDBContext repoContext)
        {
            AppDbContext = repoContext;
        }

        public IQueryable<T> FindAll()
        {
            return AppDbContext.Set<T>().AsNoTracking();
        }

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression)
        {
            return AppDbContext.Set<T>().Where(expression).AsNoTracking();
        }

        public void Create(T entity)
        {
            AppDbContext.Set<T>().Add(entity);
        }

        public void CreateRange(IEnumerable<T> entities)
        {
            AppDbContext.Set<T>().AddRange(entities);
        }

        public void Update(T entity)
        {
            AppDbContext.Set<T>().Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            AppDbContext.Set<T>().UpdateRange(entities);
        }

        public void Delete(T entity)
        {
            AppDbContext.Set<T>().Remove(entity);
        }

        public void DeleteRange(IEnumerable<T> entities)
        {
            AppDbContext.Set<T>().RemoveRange(entities);
        }



    }
}
