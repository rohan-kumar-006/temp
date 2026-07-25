using InventoryManagement.API.DTOs.Users;
using InventoryManagement.API.Models;
using InventoryManagement.API.Repositories.Interfaces;
using InventoryManagement.API.Services.Interfaces;

namespace InventoryManagement.API.Services.Implementations;

public class UserService : IUserService
{   
    private readonly IUserRepository _userRepository;
    public UserService(IUserRepository userRepository)
    {
        _userRepository=userRepository;
    }

    public async Task<UserDto> CreateStaffAsync(CreateUserDto request)
    {
        var existingUser=_userRepository.GetByEmailAsync(request.Email);

        if (existingUser != null)
        {
            throw new ArgumentException("Email Already Exists");
        }

        var user=new User
        { 
            FullName=request.FullName,
            Email=request.Email,
            PasswordHash=BCrypt.Net.BCrypt.HashPassword(request.Password),
            Role=Enums.UserRole.Staff,
            IsActive=true,
            CreatedAt=DateTime.UtcNow
        };

        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        
        return new UserDto
        {
            Id=user.Id,
            FullName=user.FullName,
            Email=user.Email,
            Role=user.Role.ToString(),
            IsActive=user.IsActive
        };
    }
}