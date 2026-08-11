using InventoryManagement.API.Common;
using InventoryManagement.API.DTOs.Dashboard;
using InventoryManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace InventoryManagement.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IDashboardService _dashboardService;
        public DashboardController(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        [HttpGet]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<ApiResponse<AdminDashboardDto>>> GetAdminDashboard()
        {
            var dashboard = await _dashboardService.GetAdminDashboardAsync();
            return Ok(
                new ApiResponse<AdminDashboardDto>(
                    true,
                    "Admin Dashboard retrieved Succesfully",
                    dashboard
                    )
                );
        }
        [HttpGet("staff")]
        [Authorize(Roles = "Staff,Admin")]
        public async Task<ActionResult<ApiResponse<StaffDashboardDto>>> GetStaffDashboard()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if(!int.TryParse(userIdValue,out int userId))
            {
                return Unauthorized();
            }
            var dashboard = await _dashboardService.GetStaffDashboardAsync(userId);

            return new ApiResponse<StaffDashboardDto>(
                true,
                "Staff Dashboard retrieved Succesfully",
                dashboard
                );
        }
    }   
}
