namespace InventoryManagement.API.DTOs.Products;

public class ProductDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string SKU { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Quantity { get; set; }

    public int ReorderLevel { get; set; }

    public string? ImageUrl { get; set; }

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}