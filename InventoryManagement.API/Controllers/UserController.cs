using InventoryManagement.API.Common;
using InventoryManagement.API.DTOs.Users;
using InventoryManagement.API.Models;
using InventoryManagement.API.Services.Implementations;
using InventoryManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace InventoryManagement.API.Controllers;

[ApiController]
[Route("/api/users")]
[Authorize(Roles ="Admin")]
public class UserController:ControllerBase
{   
    private readonly IUserService _userService;

    public UserController(IUserService userService){
        _userService=userService;
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<UserDto>>> CreateStaff(
        CreateUserDto request
    )
    {
        var user= await _userService.CreateStaffAsync(request);
        return Ok(new ApiResponse<UserDto>
        {
            Success=true,
            Message="Staff Member Created Successfully",
            Data=user
        });
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAllStaff()
    {
        var users=await _userService.GetAllStaffAsync();

        return Ok(new ApiResponse<IEnumerable<UserDto>>(
            true,
            "Staff Members Retrieved",
            users
        ));
    }
}