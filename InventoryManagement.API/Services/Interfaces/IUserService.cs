
using InventoryManagement.API.DTOs.Users;

namespace InventoryManagement.API.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateStaffAsync(CreateUserDto request);
    
    Task<IEnumerable<UserDto>> GetAllStaffAsync();

    Task<UserDto> UpdateUserAsync(int id, UpdateUserDto request);

    Task<UserDto> ToggleStatusAsync(int id);        
}