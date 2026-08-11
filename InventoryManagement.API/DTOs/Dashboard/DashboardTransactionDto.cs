using InventoryManagement.API.Enums;
namespace InventoryManagement.API.DTOs.Dashboard;

public class DashboardTransactionDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = String.Empty;

        public string SKU { get; set; } = string.Empty;
        public TransactionType Type { get; set; } 

        public int Quantity { get; set; }

        public string? PerformedBy { get; set; }=string.Empty;

        public DateTime CreatedAt { get; set; }

    }

