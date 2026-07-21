using Microsoft.EntityFrameworkCore;
using InventoryManagement.API.Models;
using Microsoft.Identity.Client;

namespace InventoryManagement.API.Data;
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }
    public DbSet<User> Users{get;set;}
    public DbSet<Product> Product{get;set;}
    public DbSet<StockTransaction> StockTransactions{get;set;}
}