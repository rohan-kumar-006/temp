using InventoryManagement.API.Common;
using InventoryManagement.API.DTOs.Auth;
using InventoryManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace InventoryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService=authService;
    }

    [HttpPost("login")]
    [EnableRateLimiting("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(LoginRequestDto request)
    {
        var response = await  _authService.LoginAsync(request);
        return Ok(
            new ApiResponse<LoginResponseDto>(
                true,
                "Login Successfull",
                response
            )
        );
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Refresh()
    {
        var response = await _authService.RefreshAsync();
        return Ok(
            new ApiResponse<LoginResponseDto>(
                true,
                "Token refreshed successfully",
                  response
                )
            );
    }
    [HttpPost("logout")]
    public async Task<ActionResult<ApiResponse<object>>> Logout()
    {
        await _authService.LogoutAsync();

        return Ok(
            new ApiResponse<object>(
                true,
                "Logout successful.",
                null
            )
        );
    }
}