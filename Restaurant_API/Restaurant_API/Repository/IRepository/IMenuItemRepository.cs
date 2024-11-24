using Restaurant_API.Models;
using System.Threading.Tasks;

namespace Restaurant_API.Repository.IRepository
{
    public interface IMenuItemRepository : IRepository<MenuItem>
    {
        Task UpdateAsync(MenuItem menuItem);
        Task<string> SaveImageAsync(IFormFile file);
        void DeleteImage(string imageName);
    }
}

