using System.ComponentModel.DataAnnotations;
using InventoryManagement.API.Enums;

namespace InventoryManagement.API.DTOs.StockTransactions;

public class CreateStockTransactionDto
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public TransactionType Type { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }
}