namespace InventoryManagement.API.DTOs.Dashboard
{
    public class AdminDashboardDto
    {
            public int TotalProducts { get; set; }
            public int LowStockProducts { get; set; }
            public int TotalStaff { get; set; }
            public int TotalStock { get; set; }
            public int StockInToday { get; set; }
            public int StockOutToday { get; set; }

            public int TransactionsToday { get; set; }

            public List<LowStockProductDto> LowStockItems { get; set; } = new();
            public List<DashboardTransactionDto> RecentTransactions { get; set; } = new();

    }
}
