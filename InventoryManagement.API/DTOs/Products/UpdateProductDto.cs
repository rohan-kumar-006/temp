using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace InventoryManagement.API.DTOs.Products;

public class UpdateProductDto
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [Range(0.01, double.MaxValue)]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue)]
    public int ReorderLevel { get; set; }

    public IFormFile? Image { get; set; }
}