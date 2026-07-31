using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.DTOs.Products.Common;
using InventoryManagement.API.Models;

namespace InventoryManagement.API.Repositories.Interfaces;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetBySkuAsync(string sku);
    Task<PagedResult<Product>> GetProductsAsync(ProductQueryParameters parameters);
    void Update(Product product);
    void Remove(Product product);
    Task SaveChangesAsync();
}