using InventoryManagement.API.Enums;

namespace InventoryManagement.API.DTOs.TransactionHistory;

public class TransactionHistoryDto
{
    public int Id { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public string PerformedBy { get; set; } = string.Empty;

    public TransactionType Type { get; set; }

    public int Quantity { get; set; }

    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }
}