
using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.DTOs.Users;

namespace InventoryManagement.API.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateStaffAsync(CreateUserDto request);

    Task<PagedResult<UserDto>> GetAllStaffAsync(
        int page,
        int pageSize,
        string? search
    );
    Task<UserDto> UpdateUserAsync(int id, UpdateUserDto request);

    Task<UserDto> ToggleStatusAsync(int id);        
}