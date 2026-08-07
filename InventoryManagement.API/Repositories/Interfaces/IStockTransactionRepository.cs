using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.DTOs.TransactionHistory;
using InventoryManagement.API.Models;

namespace InventoryManagement.API.Repositories.Interfaces;

public interface IStockTransactionRepository
{
    Task AddAsync(StockTransaction transaction);

    Task<IEnumerable<StockTransaction>> GetAllAsync();

    Task<IEnumerable<StockTransaction>> GetByProductIdAsync(int productId);
    
    Task<PagedResult<StockTransaction>> GetTransactionHistoryAsync(TransactionHistoryQueryParameters parameters);
    Task SaveChangesAsync();
}