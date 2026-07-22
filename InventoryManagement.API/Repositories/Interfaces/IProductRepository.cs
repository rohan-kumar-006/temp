using InventoryManagement.API.Models;

namespace InventoryManagement.API.Repositories.Interfaces;

public interface IProductRepository
{
    Task<IEnumerable<Product>> GetAllAsync();

    Task<Product?> GetByIdAsync(int id);

    Task AddAsync(Product product);

    void Update(Product product);
    void Delete(Product product);

    Task<bool> ExistsBySkuAsync(string sku);

    Task SaveChangesAsync();
}