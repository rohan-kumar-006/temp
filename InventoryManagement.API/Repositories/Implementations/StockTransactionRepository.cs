namespace InventoryManagement.API.Repositories.Implementations;

using InventoryManagement.API.Data;
using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.DTOs.TransactionHistory;
using InventoryManagement.API.Models;
using InventoryManagement.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

public class StockTransactionRepository : IStockTransactionRepository
{
    private readonly ApplicationDbContext _context;

    public StockTransactionRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    public async Task AddAsync(StockTransaction transaction)
    {
        await _context.StockTransactions.AddAsync(transaction);
    }

    public async Task<IEnumerable<StockTransaction>> GetAllAsync()
    {
        return await _context.StockTransactions
                                    .Include(p => p.Product)
                                    .Include(p => p.User)
                                    .ToListAsync();
    }

    public async Task<IEnumerable<StockTransaction>> GetByProductIdAsync(int productId)
    {
        return await _context.StockTransactions
            .Include(p => p.User)
            .Where(p => p.ProductId == productId)
            .ToListAsync();
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    //StockTransaction Bala Part

    public async Task<PagedResult<StockTransaction>> GetTransactionHistoryAsync(
     TransactionHistoryQueryParameters parameters)
    {
        var query = _context.StockTransactions
            .Include(t => t.Product)
            .Include(t => t.User)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.Trim().ToLower();

            query = query.Where(t =>
                t.Product!.Name.ToLower().Contains(search) ||
                t.Product!.SKU.ToLower().Contains(search));
        }
        if (parameters.Type.HasValue)
        {
            query = query.Where(t =>
                t.Type == parameters.Type.Value);
        }
        if (parameters.Date.HasValue)
        {
            var date = parameters.Date.Value.Date;
            query = query.Where(t => t.CreatedAt.Date == date);
        }
        query = query.OrderByDescending(t => t.CreatedAt);
        var totalItems = await query.CountAsync();

        var items = await query
            .Skip((parameters.Page - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();
        return new PagedResult<StockTransaction>
        {
            Items = items,
            Page = parameters.Page,
            PageSize = parameters.PageSize,
            TotalItems = totalItems,
            TotalPages = (int)Math.Ceiling(
                            totalItems / (double)parameters.PageSize)
        };
    }

}











// using Microsoft.EntityFrameworkCore;




// public class StockTransactionRepository : IStockTransactionRepository
// {
//     private readonly ApplicationDbContext _context;

//     public StockTransactionRepository(ApplicationDbContext context)
//     {
//         _context = context;
//     }

//     public async Task AddAsync(StockTransaction transaction)
//     {
//         await _context.StockTransactions.AddAsync(transaction);
//     }

//     public async Task<IEnumerable<StockTransaction>> GetAllAsync()
//     {
//         return await _context.StockTransactions
//             .Include(t => t.Product)
//             .Include(t => t.User)
//             .ToListAsync();
//     }

//     public async Task<IEnumerable<StockTransaction>> GetByProductIdAsync(int productId)
//     {
//         return await _context.StockTransactions
//             .Include(t => t.User)
//             .Where(t => t.ProductId == productId)
//             .ToListAsync();
//     }

//     public async Task SaveChangesAsync()
//     {
//         await _context.SaveChangesAsync();
//     }
// }