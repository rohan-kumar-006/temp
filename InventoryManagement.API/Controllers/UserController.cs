using System.Diagnostics;
using InventoryManagement.API.Common;
using InventoryManagement.API.DTOs.Users;
using InventoryManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateStaff(int id,UpdateUserDto request)
    {
        var user= await _userService.UpdateUserAsync(id,request);
        return Ok(new ApiResponse<UserDto>(
            true,
            "User Update Successfully",
            user
        ));
    }

    [HttpPatch("{id}/status")]
    public async Task<ActionResult<ApiResponse<UserDto>>> ToggleStatus(int id)
    {   
        var user=await _userService.ToggleStatusAsync(id);

        string message=user.IsActive ? "Staff activated successfully." : "Staff Dectivated successfully." ;
        return Ok(
            new ApiResponse<UserDto>(
            true,
            message,
            user
        ));
    }

}