using InventoryManagement.API.DTOs.Users;

namespace InventoryManagement.API.Services.Interfaces;

public interface IUserService
{
    Task<UserDto> CreateStaffAsync(CreateUserDto request);
}