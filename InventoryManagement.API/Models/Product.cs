using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.API.Models;

public class Product
{
    public int Id { get; set; }
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
    public int Quantity { get; set; }

    [Range(0, int.MaxValue)]
    public int ReorderLevel { get; set; }

    public string? ImageUrl { get; set; }

    public int CreatedByUserId { get; set; }

    public User? CreatedByUser { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
// public ICollection<StockTransaction> StockTransactions
// = new List<StockTransaction>();