using System.Numerics;
using Azure;
using InventoryManagement.API.Data;
using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.DTOs.Products.Common;
using InventoryManagement.API.Models;
using InventoryManagement.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.API.Repositories.Implementations;

public class ProductRepository : IProductRepository
{
    private readonly ApplicationDbContext _context;

    public ProductRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Product>> GetProductsAsync(ProductQueryParameters parameters)
    {
        IQueryable<Product> query = _context.Products.Include(p => p.CreatedByUser);
        if (!String.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.Trim();
            query = query.Where(p => p.Name == search || p.SKU == search);
        }

        if (parameters.MinPrice.HasValue)
        {
            query = query.Where(p => p.Price >= parameters.MinPrice.Value);
        }

        if (parameters.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= parameters.MaxPrice.Value);
        }

        if (parameters.LowStockOnly == true)
        {
            query = query.Where(p => p.Quantity <= p.ReorderLevel);
        }

        query = (parameters.SortBy ?? "name").ToLower() switch
        {
            "price" => parameters.Descending
                ? query.OrderByDescending(p => p.Price)
                : query.OrderBy(p => p.Price),

            "quantity" => parameters.Descending
                ? query.OrderByDescending(p => p.Quantity)
                : query.OrderBy(p => p.Quantity),

            "createdat" => parameters.Descending
                ? query.OrderByDescending(p => p.CreatedAt)
                : query.OrderBy(p => p.CreatedAt),

            _ =>
                parameters.Descending
                ? query.OrderByDescending(p => p.Name)
                : query.OrderBy(p => p.Name),
        };

        var totalItems = await query.CountAsync();

        var products = await query
                .Skip((parameters.Page - 1) * parameters.pageSize)
                .Take(parameters.pageSize)
                .ToListAsync();

        return new PagedResult<Product>
        {
            Items = products,
            Page = parameters.Page,
            PageSize = parameters.pageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(
                totalItems / (double)parameters.pageSize
            )
        };
    }


    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.CreatedByUser)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
    public async Task<Product?> GetBySkuAsync(string sku)
    {
        return await _context.Products
            .FirstOrDefaultAsync(p => p.SKU == sku);
    }

    public void Update(Product product)
    {
        _context.Products.Update(product);
    }

    public void Remove(Product product)
    {
        _context.Products.Remove(product);
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