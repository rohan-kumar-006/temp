using InventoryManagement.API.Configuration;
using InventoryManagement.API.DTOs.Auth;
using InventoryManagement.API.DTOs.Users;
using InventoryManagement.API.Helpers.Interfaces;
using InventoryManagement.API.Repositories.Interfaces;
using InventoryManagement.API.Services.Interfaces;
using Microsoft.CodeAnalysis.Options;
using Microsoft.Extensions.Options;

namespace InventoryManagement.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public AuthService(IUserRepository userRepository, IJwtTokenGenerator tokenGenerator, IOptions<JwtSettings> options)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _jwtSettings = options.Value;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            throw new UnauthorizedAccessException("Invalid Email or Password");
        }
        if (!user.IsActive)
        {
            throw new UnauthorizedAccessException("Your Acocunt is not Active");
        }

        bool passwordMatches = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!passwordMatches)
        {
            throw new UnauthorizedAccessException("Invalid Email or Password");
        }
        var token = _tokenGenerator.GenerateToken(user);

        return new LoginResponseDto
        {
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    
}