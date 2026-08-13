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
}