using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.DTOs.StockTransactions;
using InventoryManagement.API.DTOs.TransactionHistory;

namespace InventoryManagement.API.Services.Interfaces;
public interface IStockTransactionService
{
    Task<StockTransactionResponseDto> CreateTransactionAsync(CreateStockTransactionDto request);
    Task<PagedResult<TransactionHistoryDto>> GetTransactionHistoryAsync
            (TransactionHistoryQueryParameters parameters);
}