using InventoryManagement.API.Models;

namespace InventoryManagement.API.Repositories.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
        Task SaveChangesAsync();
    }
}
