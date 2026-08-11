namespace InventoryManagement.API.DTOs.Dashboard
{
    public class StaffDashboardDto
    {
        public int TotalProducts { get; set; }
        public int LowStockProducts { get; set; }
        public int TotalStock { get; set; }

        public List<LowStockProductDto> LowStockItems { get; set; } = new();

        public List<DashboardTransactionDto> MyRecentTransactions { get; set; } = new();

    }
}
