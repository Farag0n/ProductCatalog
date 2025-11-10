using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Interfaces;

public interface IProductRepository
{
    public Task<Product> AddProduct(Product product);
    public Task<IEnumerable<Product>> GetAllProducts();
    public Task<Product?> GetProductById(int id);
    public Task<Product?> UpdateProduct(Product product);
    public Task<Product?> DeleteProduct(int id);
}