using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.API.DTOs.Users;

public class CreateUserDto
{
    [Required]
    [MaxLength(100)]
    public string FullName{get;set;}=String.Empty;
    
    [Required]
    [EmailAddress]
    public string Email{get;set;}=String.Empty;

    [Required]
    [MinLength(6)]
    public string Password{get;set;}=String.Empty;
}