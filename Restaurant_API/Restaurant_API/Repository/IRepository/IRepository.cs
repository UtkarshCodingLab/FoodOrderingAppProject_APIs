using System.Linq.Expressions;

namespace Restaurant_API.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        // Get all records
        Task<IEnumerable<T>> GetAllAsync();

        // Get record by Id
        Task<T> GetAsync(Expression<Func<T, bool>> filter = null, string? includeProperties = null);

        // Add a new record
        Task CreateAsync(T entity);

        // Update an existing record
        Task UpdateAsync(T entity);

        // Remove a record
        Task RemoveAsync(T entity);

        // Save changes to the database
        Task SaveAsync();
    }
}
