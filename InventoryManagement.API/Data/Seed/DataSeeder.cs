using InventoryManagement.API.Enums;
using InventoryManagement.API.Models;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.API.Data.Seed;

public class DataSeeder
{
    public static async Task SeedAdminAsync(
        ApplicationDbContext context,
        IConfiguration configuration)
    {
        var adminExists = await context.Users
            .AnyAsync(u => u.Role == UserRole.Admin);

        if (adminExists)
            return;

        var email = configuration["SeedAdmin:Email"];
        var password = configuration["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(password))
        {
            throw new InvalidOperationException(
                "Seed admin credentials are not configured."
            );
        }

        var admin = new User
        {
            FullName = "System Administrator",
            Email = email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            Role = UserRole.Admin,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        context.Users.Add(admin);

        await context.SaveChangesAsync();
    }
}