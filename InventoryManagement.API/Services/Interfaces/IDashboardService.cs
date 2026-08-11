using InventoryManagement.API.DTOs.Dashboard;

namespace InventoryManagement.API.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<AdminDashboardDto> GetAdminDashboardAsync();
        Task<StaffDashboardDto> GetStaffDashboardAsync(int userId);
    }
}
