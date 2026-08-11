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
    Task<bool> HasTransactionsAsync(int productId);

    Task DeleteAsync(Product product);
    // Task UpdateAsync(Product product);
    void Update(Product product);
    void Remove(Product product);

    //dashboard ke liye
    Task<int> GetTotalProductsAsync();

    Task<int> GetLowStockProductCountAsync();

    Task<int> GetTotalStockAsync();

    Task<IEnumerable<Product>> GetLowStockProductsAsync(int count);

    Task SaveChangesAsync();
}