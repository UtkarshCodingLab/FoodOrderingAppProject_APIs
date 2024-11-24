using Microsoft.EntityFrameworkCore;
using Restaurant_API.Data;
using Restaurant_API.Repository.IRepository;
using System.Linq.Expressions;

namespace Restaurant_API.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        private readonly DbSet<T> _dbSet;

        public Repository(ApplicationDbContext db)
        {
            _db = db;
            _dbSet = _db.Set<T>();
        }

        // Get all records
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        // Get a record by Id or filter using a lambda expression
        public async Task<T> GetAsync(Expression<Func<T, bool>> filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            // Handling includes (if any)
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProperty in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            return await query.FirstOrDefaultAsync();
        }

        // Add a new entity
        public async Task CreateAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
            await SaveAsync();
        }

        // Update an existing entity
        public async Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            await SaveAsync();
        }

        // Remove an entity
        public async Task RemoveAsync(T entity)
        {
            _dbSet.Remove(entity);
            await SaveAsync();
        }

        // Save changes to the database
        public async Task SaveAsync()
        {
            await _db.SaveChangesAsync();
        }
    }
}

