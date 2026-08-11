using InventoryManagement.API.DTOs.Dashboard;
using InventoryManagement.API.Repositories.Interfaces;
using InventoryManagement.API.Services.Interfaces;

namespace InventoryManagement.API.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IProductRepository _productRepository;
        private readonly IStockTransactionRepository _stockTransactionRepository;
        private readonly IUserRepository _userRepository;

        public DashboardService(IProductRepository productRepository, IStockTransactionRepository stockTransactionRepository, IUserRepository userRepository)
        {
            _productRepository = productRepository;
            _stockTransactionRepository = stockTransactionRepository;
            _userRepository = userRepository;
        }

        public async Task<AdminDashboardDto> GetAdminDashboardAsync()
        {
            var totalProducts = await _productRepository.GetTotalProductsAsync();
            var lowStockCount = await _productRepository.GetLowStockProductCountAsync();
            var totalStock = await _productRepository.GetTotalStockAsync();
            var totalStaff=await _userRepository.GetStaffCountAsync();
            var stockInToday =await _stockTransactionRepository.GetStockInTodayAsync();
            var stockOutToday =await _stockTransactionRepository.GetStockOutTodayAsync();
            var transactionsToday =await _stockTransactionRepository.GetTransactionCountTodayAsync();

            var lowStockProducts = await _productRepository.GetLowStockProductsAsync(5);
            // ye Product retrun krega pr frontend ko mujhe LowStocKTxnDto bhejna hai na
            var lowStockItems = lowStockProducts.Select(x => new LowStockProductDto
            {
                Id = x.Id,
                Name = x.Name,
                SKU = x.SKU,
                Quantity = x.Quantity,
                ReorderLevel = x.ReorderLevel
            }).ToList();

            var recentTransactions = await _stockTransactionRepository.GetRecentTransactionsAsync(5);
            var transactions = recentTransactions.Select(t => new DashboardTransactionDto
            {
                Id = t.Id,
                ProductName = t.Product!.Name,
                SKU = t.Product.SKU,
                Type=t.Type,
                Quantity = t.Quantity,
                PerformedBy = t.User?.FullName,
                CreatedAt = t.CreatedAt
            }).ToList();

            return new AdminDashboardDto
            {
                TotalProducts = totalProducts,
                LowStockProducts = lowStockCount,
                TotalStaff = totalStaff,
                TotalStock = totalStock,
                StockInToday = stockInToday,
                StockOutToday = stockOutToday,
                TransactionsToday = transactionsToday,
                LowStockItems= lowStockItems,
                RecentTransactions=transactions
            };
        }

        public async Task<StaffDashboardDto> GetStaffDashboardAsync(int userId)
        {
            var totalProducts = await _productRepository.GetTotalProductsAsync();
            var lowStockCount = await _productRepository.GetLowStockProductCountAsync();
            var totalStock = await _productRepository.GetTotalStockAsync();

            var lowStockProducts = await _productRepository.GetLowStockProductsAsync(5);
            var lowStockItems = lowStockProducts.Select(x => new LowStockProductDto
            {
                Id = x.Id,
                Name = x.Name,
                SKU = x.SKU,
                Quantity = x.Quantity,
                ReorderLevel = x.ReorderLevel
            }).ToList();

            var myRecentTransactions = await _stockTransactionRepository.GetMyRecentTransactionsAsync(userId, 5);
            var myTransactionRequired = myRecentTransactions
               .Select(t => new DashboardTransactionDto
               {
                   Id = t.Id,
                   ProductName = t.Product!.Name,
                   SKU = t.Product.SKU,
                   Type = t.Type,
                   Quantity = t.Quantity,
                   CreatedAt = t.CreatedAt
               })
               .ToList();

            return new StaffDashboardDto
            {
                TotalProducts = totalProducts,
                LowStockProducts = lowStockCount,
                TotalStock = totalStock,
                LowStockItems= lowStockItems,
                MyRecentTransactions = myTransactionRequired
            };
        }
    }
}
