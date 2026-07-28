using InventoryManagement.API.Models;

namespace InventoryManagement.API.Repositories.Interfaces;

public interface IProductRepository
{
    Task AddAsync(Product product);
    Task<Product?> GetByIdAsync(int id);
    Task<Product?> GetBySkuAsync(string sku);

    void Update(Product product);
    void Remove(Product product);
    Task SaveChangesAsync();
}