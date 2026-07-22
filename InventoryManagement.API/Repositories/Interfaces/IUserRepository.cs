using InventoryManagement.API.Models;

namespace InventoryManagement.API.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByIdAsync(int id);

    Task<IEnumerable<User>> GetAllSync();

    Task AddAsync(User user);
    void Update(User user);

    Task SaveChangesAsync();
}