using DevTalles.Ecommerce.WebAPI.Data;
using DevTalles.Ecommerce.WebAPI.Models;
using DevTalles.Ecommerce.WebAPI.Repository.IRepository;

namespace DevTalles.Ecommerce.WebAPI.Repository
{
    public class CategoryRepository(ApplicationDbContext db) : ICategoryRepository
    {
        private readonly ApplicationDbContext _db = db;

        public ICollection<Category> GetCategories()
        {
            return _db.Categories.OrderBy(c => c.Name).ToList();
        }

        public Category GetCategory(int categoryId)
        {
            return _db.Categories.FirstOrDefault(c => c.Id == categoryId) ??
                   throw new InvalidOperationException($"Category with ID {categoryId} not found.");
        }

        public bool CategoryExists(int id)
        {
            return _db.Categories.Any(c => c.Id == id);
        }

        public bool CategoryExists(string name)
        {
            return _db.Categories.Any(c => c.Name.ToLower().Trim() == name.ToLower().Trim());
        }

        public bool CreateCategory(Category category)
        {
            category.CreationDate = DateTime.Now;
            _db.Categories.Add(category);
            return Save();
        }

        public bool UpdateCategory(Category category)
        {
            category.CreationDate = DateTime.Now;
            _db.Categories.Update(category);
            return Save();
        }

        public bool DeleteCategory(Category category)
        {
            ArgumentNullException.ThrowIfNull(category);

            _db.Categories.Remove(category);
            return Save();
        }

        public bool Save()
        {
            return _db.SaveChanges() > 0;
        }
    }
}
