using InventoryManagement.API.DTOs.Products;

namespace InventoryManagement.API.DTOs.StockTransactions;
public class StockTransactionResponseDto
{
    public ProductDto Product { get; set; } = null!;
}