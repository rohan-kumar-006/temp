using InventoryManagement.API.Data;
using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.Enums;
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

    public async Task<PagedResult<User>> GetAllStaffAsync(int page,int pageSize,string? search)
    {
        var query=_context.Users.Where(u=>u.Role==UserRole.Staff);

        if (!string.IsNullOrWhiteSpace(search))
        {   
            search= search.Trim();
            query = query.Where(u =>
            u.FullName.Contains(search) || u.Email.Contains(search)); 
        }

        var totalItems=await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

        var users = await query.OrderByDescending(u => u.CreatedAt)
                            .Skip((page - 1) * pageSize)
                            .Take(pageSize)
                            .ToListAsync();
        return new PagedResult<User>
        {
            Items = users,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems,
            TotalPages = totalPages
        };
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