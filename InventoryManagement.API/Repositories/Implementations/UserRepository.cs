using InventoryManagement.API.Data;
using InventoryManagement.API.Models;
using InventoryManagement.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.API.Repositories.Implementations;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetAllStaffAsync()
    {
        return await _context.Users
            .Where(u=>u.Role==Enums.UserRole.Staff)
            .OrderByDescending(u=>u.CreatedAt)
            .ToListAsync();
    }

    public async Task<int> GetStaffCountAsync()
    {  //isme maine inactive and active dono rakh diye the
        return await _context.Users.CountAsync(u=>u.Role==Enums.UserRole.Staff);
    }

    public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    public void Update(User user)
    {
        _context.Users.Update(user);
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}