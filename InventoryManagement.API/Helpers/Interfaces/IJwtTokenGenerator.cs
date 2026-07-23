using InventoryManagement.API.Models;

namespace InventoryManagement.API.Helpers.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}