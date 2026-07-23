using InventoryManagement.API.Data;
using InventoryManagement.API.Models;
using InventoryManagement.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.API.Repositories.Implementations;

public class ProductRepository : IProductRepository{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context=context;
    }
    
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products.Include(p=>p.CreatedByUser).ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.CreatedByUser)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }
    public void Update(Product product)
    {
        _context.Products.Update(product);
    }

    public void Delete(Product product)
    {
        _context.Products.Remove(product);
    }
    public async Task<bool> ExistsBySkuAsync(string sku)
    {
        return await _context.Products.AnyAsync(p=>p.SKU==sku);
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}






















// using InventoryManagement.API.Data;
// using InventoryManagement.API.Models;
// using InventoryManagement.API.Repositories.Interfaces;
// using Microsoft.EntityFrameworkCore;

// namespace InventoryManagement.API.Repositories.Implementations;

// public class ProductRepository : IProductRepository
// {
//     private readonly ApplicationDbContext _context;

//     public ProductRepository(ApplicationDbContext context)
//     {
//         _context = context;
//     }

//     public async Task<IEnumerable<Product>> GetAllAsync()
//     {
//         return await _context.Products
//             .Include(p => p.CreatedByUser)
//             .ToListAsync();
//     }

//     public async Task<Product?> GetByIdAsync(int id)
//     {
//         return await _context.Products
//             .Include(p => p.CreatedByUser)
//             .FirstOrDefaultAsync(p => p.Id == id);
//     }

//     public async Task AddAsync(Product product)
//     {
//         await _context.Products.AddAsync(product);
//     }

//     public void Update(Product product)
//     {
//         _context.Products.Update(product);
//     }

//     public void Delete(Product product)
//     {
//         _context.Products.Remove(product);
//     }

//     public async Task<bool> ExistsBySkuAsync(string sku)
//     {
//         return await _context.Products
//             .AnyAsync(p => p.SKU == sku);
//     }

//     public async Task SaveChangesAsync()
//     {
//         await _context.SaveChangesAsync();
//     }
// }