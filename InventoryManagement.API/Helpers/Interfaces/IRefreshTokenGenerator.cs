namespace InventoryManagement.API.Helpers.Interfaces
{
    public interface IRefreshTokenGenerator
    {
        string GenerateToken();

        string HashToken(string token);
    }
}
