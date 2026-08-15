using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.DTOs.Users;
using InventoryManagement.API.Enums;
using InventoryManagement.API.Models;
using InventoryManagement.API.Repositories.Interfaces;
using InventoryManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Caching.Memory;

namespace InventoryManagement.API.Services.Implementations;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserService> _logger;
    private readonly IMemoryCache _cache;
    public UserService(IUserRepository userRepository, ILogger<UserService> logger,IMemoryCache cache)
    {
        _userRepository = userRepository;
        _logger = logger;
        _cache = cache;
    }

    public async Task<UserDto> CreateStaffAsync(CreateUserDto request)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser != null)
        {
            throw new ArgumentException("Email Already Exists");
        }

        var user = new User
        {
            FullName = request.FullName,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role = Enums.UserRole.Staff,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        _cache.Remove("admin-dashboard");
        _logger.LogInformation(
            "Staff account created. UserId: {UserId}, Email: {Email}",
            user.Id,
            user.Email
        );


        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive
        };
    }

    public async Task<PagedResult<UserDto>> GetAllStaffAsync(int page, int pageSize, string? search)
    {
        if (page < 1)
        {
            page = 1;
        }
        if (pageSize < 1)
        {
            pageSize = 10;
        }
        if (pageSize > 50)
        {
            pageSize = 50;
        }
        var result = await _userRepository.GetAllStaffAsync(
            page,
            pageSize,
            search
        );

        return new PagedResult<UserDto>
        {
            Items = result.Items.Select(u => new UserDto
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email,
                Role = u.Role.ToString(),
                IsActive = u.IsActive
            }),
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };
    }
    public async Task<UserDto> UpdateUserAsync(int id, UpdateUserDto request)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            throw new KeyNotFoundException(
                "Staff member not found."
            );
        }

        if (user.Role != Enums.UserRole.Staff)
        {
            throw new ArgumentException(
                "Only staff members can be updated."
            );
        }

        var existingUser = await _userRepository.GetByEmailAsync(request.Email);

        if (existingUser != null && existingUser.Id != id)
        {
            throw new Exception("Email Already Exists");
        }

        user.Email = request.Email;
        user.FullName = request.FullName;

        _userRepository.Update(user);
        await _userRepository.SaveChangesAsync();
        _cache.Remove("admin-dashboard");
        _logger.LogInformation(
            "Staff account updated. UserId: {UserId}, Email: {Email}",
            user.Id,
            user.Email
        );
        return new UserDto
        {
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString(),
            IsActive = user.IsActive
        };
    }

    public async Task<UserDto> ToggleStatusAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);

        if (user == null)
        {
            throw new KeyNotFoundException(
                "Staff member not found."
            );
        }

        if (user.Role != UserRole.Staff)
        {
            throw new ArgumentException(
                "Only staff members can be activated or deactivated."
            );
        }
        user.IsActive = !user.IsActive;
        await _userRepository.SaveChangesAsync();
        _cache.Remove("admin-dashboard");
        _logger.LogInformation(
            "Staff account status changed. UserId: {UserId}, Email: {Email}, Status: {Status}",
            user.Id,
            user.Email,
            user.IsActive ? "Active" : "Inactive"
        );
        return UserMapper.ToDto(user);
    }

}