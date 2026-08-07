using InventoryManagement.API.Configuration;
using InventoryManagement.API.DTOs.Auth;
using InventoryManagement.API.Helpers.Interfaces;
using InventoryManagement.API.Repositories.Interfaces;
using InventoryManagement.API.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace InventoryManagement.API.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly JwtSettings _jwtSettings;
    private readonly IJwtTokenGenerator _tokenGenerator;
    private readonly ILogger<AuthService> _logger;

    public AuthService(IUserRepository userRepository, IJwtTokenGenerator tokenGenerator,
    IOptions<JwtSettings> options, ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _jwtSettings = options.Value;
        _logger = logger;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email);

        if (user == null)
        {
            _logger.LogWarning("Failed login attempt. Email: {Email}. Reason: User not found.", request.Email);
            throw new UnauthorizedAccessException("Invalid Email or Password");
        }
        if (!user.IsActive)
        {
            _logger.LogWarning("Login attempt for inactive account. Email: {Email}", request.Email);
            throw new UnauthorizedAccessException("Your account is not active.");
        }

        bool passwordMatches = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);

        if (!passwordMatches)
        {
            _logger.LogWarning(
                "Failed login attempt. Email: {Email}. Reason: Incorrect password.",
                request.Email
            );

            throw new UnauthorizedAccessException("Invalid Email or Password");
        }
        var token = _tokenGenerator.GenerateToken(user);

        _logger.LogInformation(
            "User login successful. UserId: {UserId}, Email: {Email}, Role: {Role}",
            user.Id,
            user.Email,
            user.Role
        );

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