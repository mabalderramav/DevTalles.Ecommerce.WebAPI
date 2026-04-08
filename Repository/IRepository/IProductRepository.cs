using DevTalles.Ecommerce.WebAPI.Models;

namespace DevTalles.Ecommerce.WebAPI.Repository.IRepository;

public interface IProductRepository
{
    ICollection<Product> GetProducts();
    ICollection<Product> GetProductsByCategory(int categoryId);
    ICollection<Product> SearchProduct(string name);
    Product? GetProduct(int productId);
    bool BuyProduct(string productName, int quantity);
    bool ProductExists(int id);
    bool ProductExists(string name);
    bool CreateProduct(Product product);
    bool UpdateProduct(Product product);
    bool DeleteProduct(Product product);
    bool Save();
}