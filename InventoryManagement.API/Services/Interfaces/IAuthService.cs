namespace InventoryManagement.API.Services.Interfaces;
using InventoryManagement.API.DTOs.Auth;

public interface IAuthService
{
    Task<LoginResponseDto> LoginAsync(LoginRequestDto request);

    Task<LoginResponseDto> RefreshAsync();
    Task LogoutAsync();
}