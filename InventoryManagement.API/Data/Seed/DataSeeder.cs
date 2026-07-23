using InventoryManagement.API.Models;
using InventoryManagement.API.Enums;
namespace InventoryManagement.API.Data.Seed;

public class DataSeeder
{
    public static async Task SeedAdminAsync(ApplicationDbContext context)
    {
        if(context.Users.Any()) return;

        var admin=new User
        {
            FullName = "System Administrator",
            Email = "admin@inventory.com",
            PasswordHash=BCrypt.Net.BCrypt.HashPassword("Admin@123"),
            Role=UserRole.Admin,
            IsActive=true,
            CreatedAt=DateTime.UtcNow
        };
        context.Users.Add(admin);
        await context.SaveChangesAsync();
    }
}