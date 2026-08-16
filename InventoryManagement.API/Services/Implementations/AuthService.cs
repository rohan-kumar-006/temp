using InventoryManagement.API.Configuration;
using InventoryManagement.API.DTOs.Auth;
using InventoryManagement.API.Helpers.Interfaces;
using InventoryManagement.API.Models;
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
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IRefreshTokenGenerator _refreshTokenGenerator;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IUserRepository userRepository, IJwtTokenGenerator tokenGenerator,
    IRefreshTokenRepository refreshTokenRepository, IRefreshTokenGenerator refreshTokenGenerator, IOptions<JwtSettings> options,
    ILogger<AuthService> logger, IHttpContextAccessor httpContextAccessor)
    {
        _userRepository = userRepository;
        _tokenGenerator = tokenGenerator;
        _refreshTokenRepository = refreshTokenRepository;
        _refreshTokenGenerator = refreshTokenGenerator;
        _jwtSettings = options.Value;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
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
        var accessToken = _tokenGenerator.GenerateToken(user);
        var refreshToken = _refreshTokenGenerator.GenerateToken();
        var refreshTokenHash = _refreshTokenGenerator.HashToken(refreshToken);

        var refreshTokenEntity = new RefreshToken
        {
            TokenHash = refreshTokenHash,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),

        };
        await _refreshTokenRepository.AddAsync(refreshTokenEntity);
        await _refreshTokenRepository.SaveChangesAsync();

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            Expires = DateTime.UtcNow.AddDays(7)
        };

        _httpContextAccessor.HttpContext!.Response.Cookies.Append(
            "refreshToken",
            refreshToken,
            cookieOptions
            );

        _logger.LogInformation(
            "User login successful. UserId: {UserId}, Email: {Email}, Role: {Role}",
            user.Id,
            user.Email,
            user.Role
        );

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
            FullName = user.FullName,
            Email = user.Email,
            Role = user.Role.ToString()
        };
    }

    public async Task<LoginResponseDto> RefreshAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;

        if (httpContext == null)
        {
            throw new UnauthorizedAccessException(
                "Invalid Refresh Request");
        }

        var refreshToken = httpContext.Request.Cookies["refreshToken"];

        if (string.IsNullOrEmpty(refreshToken))
        {
            throw new UnauthorizedAccessException(
                "Refresh Token not found"
                );
        }

        var tokenHash = _refreshTokenGenerator.HashToken(refreshToken);
        var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

        if (storedToken == null)
        {
            throw new UnauthorizedAccessException(
                "Invalid refresh token"
                );
        }
        if (storedToken.RevokedAt != null)
        {
            throw new UnauthorizedAccessException(
                "Refresh token has been revoked."
            );
        }

        if (storedToken.ExpiresAt <= DateTime.UtcNow)
        {
            throw new UnauthorizedAccessException(
               "Refresh Token Expired"
               );
        }
        var user = await _userRepository.GetByIdAsync(storedToken.UserId);

        if (user == null || !user.IsActive)
        {
            throw new UnauthorizedAccessException(
               "User is not Active"
             );
        }
        storedToken.RevokedAt = DateTime.UtcNow;

        var accessToken = _tokenGenerator.GenerateToken(user);
        var newRefreshToken = _refreshTokenGenerator.GenerateToken();
        var newRefreshTokenHash = _refreshTokenGenerator.HashToken(newRefreshToken);

        var newRefreshTokenEntity = new RefreshToken
        {
            TokenHash = newRefreshTokenHash,
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);
        await _refreshTokenRepository.SaveChangesAsync();

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None,
            Path = "/",
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        };
        httpContext.Response.Cookies.Append(
            "refreshToken",
            newRefreshToken,
            cookieOptions
        );

        _logger.LogInformation("Refresh token used successfully. UserId: {UserId}", user.Id);
        return new LoginResponseDto
        {
            AccessToken = accessToken,

            ExpiresAt =
            DateTime.UtcNow.AddMinutes(
                _jwtSettings.DurationInMinutes
            ),

            FullName = user.FullName,

            Email = user.Email,

            Role = user.Role.ToString()
        };
    }
    public async Task LogoutAsync()
    {
        var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["refreshToken"];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return;
        }
        var tokenHash = _refreshTokenGenerator.HashToken(refreshToken);
        var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

        if (storedToken != null && storedToken.RevokedAt == null)
        {
            storedToken.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.SaveChangesAsync();
        }
        _httpContextAccessor.HttpContext?.Response.Cookies.Delete(
            "refreshToken",
            new CookieOptions
            {
                Path = "/",
                Secure = true,
                HttpOnly = true,
                SameSite = SameSiteMode.None
            }
);
    }
}