using Vectarr.Palooza.Context;
using Vectarr.Palooza.Model;
using Microsoft.EntityFrameworkCore;
using Vectarr.Palooza.DataAccess;


namespace Vectarr.Palooza.DataAccess.Repositories
{
    public sealed class GenericRepository<T> : IGenericRepository<T> where T : class, IBaseModel, new()
    {
        private readonly VectarrPostgresDBContext _context;

        internal GenericRepository(VectarrPostgresDBContext context)
        {
            _context = context;
        }

        public IQueryable<T> GetQueryable(bool includeRemoved = false)
        {
            var query = _context.Set<T>().AsQueryable();
            query = query.Where(x => includeRemoved || x.IsRemoved == false);
            return query;
        }

        public T GetById(int? id)
        {
            if (!id.HasValue)
            {
                return null;
            }

            return _context.Set<T>().FirstOrDefault(x => x.IsRemoved == false && x.Id == id);
        }

        public T Insert(T entity)
        {
            _context.Set<T>().Add(entity);
            _context.SaveChanges();
            return entity;
        }

        public void Update(T entity)
        {
            entity.LastModified = DateTime.UtcNow;
            _context.SaveChanges();
        }

        public void Update(List<T> entities)
        {
            entities.ForEach(x =>
            {
                x.LastModified = DateTime.UtcNow;
            });
            _context.SaveChanges();
        }

        public void Delete(T entity)
        {
            if (_context.Entry(entity).State != EntityState.Detached)
            {
                _context.Set<T>().Remove(entity);
                _context.SaveChanges();
            }
        }

        public void Delete(List<T> entities)
        {
            entities.ForEach(e =>
            {
                if (_context.Entry(e).State != EntityState.Detached)
                {
                    _context.Set<T>().Remove(e);
                }
            });
            _context.SaveChanges();
        }
    }
}
