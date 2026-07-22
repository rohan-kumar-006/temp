namespace InventoryManagement.API.Repositories.Implementations;

using InventoryManagement.API.Data;
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
                                    .Include(p=>p.User)
                                    .ToListAsync();
    }

    public async Task<IEnumerable<StockTransaction>> GetByProductIdAsync(int productId)
    {
        return await _context.StockTransactions
            .Include(p=>p.User)
            .Where(p=>p.ProductId==productId)
            .ToListAsync();
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
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