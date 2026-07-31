using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.Models;

namespace InventoryManagement.API.Helpers.Implementations;
public class ProductMapper
{
    public static ProductDto MapToProductDto(Product product)
    {
        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            SKU = product.SKU,
            Description = product.Description,
            Price = product.Price,
            Quantity = product.Quantity,
            ReorderLevel = product.ReorderLevel,
            ImageUrl = product.ImageUrl,
            CreatedBy=product.CreatedByUser?.FullName?? "",
            CreatedAt=product.CreatedAt,
        };
    }
}