namespace InventoryManagement.API.Services.Interfaces;
using InventoryManagement.API.DTOs.Auth;

public interface IAuthServices
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}