namespace InventoryManagement.API.Repositories.Implementations;

using InventoryManagement.API.Data;
using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.DTOs.TransactionHistory;
using InventoryManagement.API.Enums;
using InventoryManagement.API.Helpers.Implementations;
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
            var (startUtc, endUtc) =
                DateTimeHelper.GetUtcRangeForIndiaDate(parameters.Date.Value);

            query = query.Where(t =>
                t.CreatedAt >= startUtc &&
                t.CreatedAt < endUtc);
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

    public async Task<int> GetStockInTodayAsync()
    {
        //var today = DateTime.UtcNow.Date;
        //var tomorrow = today.AddDays(1);

        var (startUtc, endUtc) = DateTimeHelper.GetTodayUtcRange();

        return await _context.StockTransactions
            .Where(t => t.CreatedAt >= startUtc && t.CreatedAt < endUtc
            && t.Type == TransactionType.In)
            .SumAsync(t => t.Quantity);
    }
    public async Task<int> GetStockOutTodayAsync()
    {
        var (startUtc, endUtc) = DateTimeHelper.GetTodayUtcRange();
        //var today = DateTime.UtcNow.Date;
        //var tomorrow = today.AddDays(1);

        return await _context.StockTransactions
            .Where(t => t.CreatedAt >= startUtc && t.CreatedAt < endUtc
            && t.Type == TransactionType.Out)
            .SumAsync(t => t.Quantity);
    }
    public async Task<int> GetTransactionCountTodayAsync()
    {
        var (startUtc, endUtc) = DateTimeHelper.GetTodayUtcRange();
        //var today = DateTime.UtcNow.Date;
        //var tomorrow = today.AddDays(1);

        return await _context.StockTransactions
            .CountAsync(t =>
                t.CreatedAt >= startUtc &&
                t.CreatedAt < endUtc);
    }

    public async Task<IEnumerable<StockTransaction>> GetRecentTransactionsAsync(int count)
    {
        return await _context.StockTransactions
            .Include(t => t.Product)
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .Take(count).ToArrayAsync();
    }
    public async Task<IEnumerable<StockTransaction>> GetMyRecentTransactionsAsync(
        int userId,
        int count)
    {
        return await _context.StockTransactions
        .Include(t => t.Product)
        .Where(t => t.UserId == userId)
        .OrderByDescending(t => t.CreatedAt)
        .Take(count)
        .ToListAsync();
    }
}