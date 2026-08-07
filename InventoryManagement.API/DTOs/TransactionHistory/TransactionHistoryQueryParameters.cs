using InventoryManagement.API.Enums;

namespace InventoryManagement.API.DTOs.TransactionHistory;

public class TransactionHistoryQueryParameters
{

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? Search { get; set; }
    public TransactionType? Type { get; set; }
    public DateTime? Date { get; set; }
}