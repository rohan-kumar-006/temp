using System.ComponentModel.DataAnnotations;
using InventoryManagement.API.Enums;

namespace InventoryManagement.API.Models;
public class User
{
    public int Id {get;set;}
    [Required]
    [MaxLength(100)]
    public string FullName{get;set;}=string.Empty;
    [Required]
    [EmailAddress]
    public string Email{get;set;}=string.Empty;
    [Required]
    public string PasswordHash{get;set;}=string.Empty;
    public UserRole Role{get;set;}
    public bool IsActive{get;set;}
    public DateTime CreatedAt{get; set;}

}
    // public ICollection<Product> Products
    //     = new List<Product>();
    // public ICollection<StockTransaction> StockTransactions
    // = new List<StockTransaction>();