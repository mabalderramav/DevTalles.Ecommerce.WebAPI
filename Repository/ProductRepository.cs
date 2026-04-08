using DevTalles.Ecommerce.WebAPI.Data;
using DevTalles.Ecommerce.WebAPI.Models;
using DevTalles.Ecommerce.WebAPI.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace DevTalles.Ecommerce.WebAPI.Repository;

public class ProductRepository(ApplicationDbContext db) : IProductRepository
{
    private readonly ApplicationDbContext _db = db;

    public ICollection<Product> GetProducts()
    {
        return _db.Products.Include(p => p.Category).OrderBy(p => p.Name).ToList();
    }

    public ICollection<Product> GetProductsByCategory(int categoryId)
    {
        return categoryId <= 0
            ? []
            : _db.Products.Where(p => p.CategoryId == categoryId).OrderBy(p => p.Name).ToList();
    }

    public ICollection<Product> SearchProduct(string name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? []
            : _db.Products
                .Where(p => p.Name.ToLower().Trim().Contains(name.ToLower().Trim()))
                .OrderBy(p => p.Name)
                .ToList();
    }

    public Product? GetProduct(int productId)
    {
        return productId <= 0 ? null : _db.Products.Include(p => p.Category).FirstOrDefault(p => p.ProductId == productId);
    }

    public bool BuyProduct(string productName, int quantity)
    {
        if (string.IsNullOrWhiteSpace(productName) || quantity <= 0)
        {
            return false;
        }

        var product = _db.Products.FirstOrDefault(p => p.Name.ToLower().Trim() == productName.ToLower().Trim());
        if (product == null || product.Stock < quantity)
        {
            return false; // Product isn't found or insufficient stock
        }

        product.Stock -= quantity;
        _db.Products.Update(product);
        return Save();
    }

    public bool ProductExists(int id)
    {
        return id > 0 && _db.Products.Any(p => p.ProductId == id);
    }

    public bool ProductExists(string name)
    {
        return !string.IsNullOrWhiteSpace(name) &&
               _db.Products.Any(p => p.Name.ToLower().Trim() == name.ToLower().Trim());
    }

    public bool CreateProduct(Product? product)
    {
        if (product == null)
        {
            return false;
        }

        product.CreationDate = DateTime.Now;
        product.UpdateDate = DateTime.Now;
        _db.Products.Add(product);
        return Save();
    }

    public bool UpdateProduct(Product? product)
    {
        if (product == null)
        {
            return false;
        }

        product.UpdateDate = DateTime.Now;
        product.Stock = product.Stock > 0 ? product.Stock : 0;
        product.Stock = product.Stock < 10000 ? product.Stock : 10000;
        _db.Products.Update(product);
        return Save();
    }

    public bool DeleteProduct(Product? product)
    {
        if (product == null)
        {
            return false;
        }

        _db.Products.Remove(product);
        return Save();
    }

    public bool Save()
    {
        return _db.SaveChanges() > 0;
    }
}