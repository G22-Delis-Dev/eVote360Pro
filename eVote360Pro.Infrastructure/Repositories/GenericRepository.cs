using eVote360Pro.Domain.Entities;
using eVote360Pro.Domain.Interfaces.Repositories; 
using eVote360Pro.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace eVote360Pro.Infrastructure.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
            {
                return await _dbSet.ToListAsync();
            }
            return await _dbSet.Where(predicate).ToListAsync();
        }

        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
            {
                return await _dbSet.FirstOrDefaultAsync();
            }
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }
        public virtual async Task<bool> ExistsAsync(Expression<Func<T, bool>>? predicate = null)
        {
            if (predicate == null)
            {
                return await _dbSet.AnyAsync();
            }
            return await _dbSet.AnyAsync(predicate);
        }
        public virtual async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        {
            // Si no nos pasan un filtro (es nulo), contamos todos los registros de la tabla
            if (predicate == null)
            {
                return await _dbSet.CountAsync();
            }

            // Si sí nos pasan un filtro, contamos solo los que cumplan la condición
            return await _dbSet.CountAsync(predicate);
        }
        public virtual async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public virtual void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public virtual void Remove(T entity)
        {
            // Lógica de eliminación lógica
            entity.Activo = false;
            entity.FechaModificacion = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public virtual void RemoveRange(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                entity.Activo = false;
                entity.FechaModificacion = DateTime.UtcNow;
            }
            _dbSet.UpdateRange(entities);
        }
    }
}