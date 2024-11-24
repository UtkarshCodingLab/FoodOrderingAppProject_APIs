using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Restaurant_API.Data;
using Restaurant_API.Models;
using Restaurant_API.Repository.IRepository;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Restaurant_API.Repository
{
    public class MenuItemRepository : Repository<MenuItem>, IMenuItemRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public MenuItemRepository(ApplicationDbContext db, IWebHostEnvironment webHostEnvironment) : base(db)
        {
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task UpdateAsync(MenuItem menuItem)
        {
            _db.MenuItems.Update(menuItem);
            await _db.SaveChangesAsync();
        }

        public async Task<string> SaveImageAsync(IFormFile file)
        {
            var allowedFileExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var ext = Path.GetExtension(file.FileName).ToLower();

            if (!allowedFileExtensions.Contains(ext) || file.Length > 1 * 1024 * 1024)
                return null;

            var contentPath = _webHostEnvironment.ContentRootPath;
            var path = Path.Combine(contentPath, "images/menuitem");

            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            var fileName = $"{Guid.NewGuid()}{ext}";
            var fileNameWithPath = Path.Combine(path, fileName);

            using var stream = new FileStream(fileNameWithPath, FileMode.Create);
            await file.CopyToAsync(stream);

            return fileName;
        }

        public void DeleteImage(string imageName)
        {
            if (string.IsNullOrEmpty(imageName)) return;

            var path = Path.Combine(_webHostEnvironment.ContentRootPath, "images/menuitem", imageName);

            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
