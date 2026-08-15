using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.Models;

namespace InventoryManagement.API.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);

    Task<PagedResult<User>> GetAllStaffAsync(
        int page, int pageSize, string? search);

    Task<int> GetStaffCountAsync();
    Task AddAsync(User user);
    void Update(User user);

    Task SaveChangesAsync();
}