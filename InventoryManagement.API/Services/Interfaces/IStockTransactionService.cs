using InventoryManagement.API.DTOs.StockTransactions;

namespace InventoryManagement.API.Services.Interfaces;
public interface IStockTransactionService
{
    Task<StockTransactionResponseDto> CreateTransactionAsync(CreateStockTransactionDto request);
}