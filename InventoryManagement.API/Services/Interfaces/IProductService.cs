
using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.DTOs.Products.Common;

namespace InventoryManagement.API.Services.Interfaces;

public interface IProductService
{
    Task<ProductDto> CreateProductAsync(CreateProductDto request, int createdByUserId);
    Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParameters parameters);
    Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto request);
    Task DeleteProductAsync(int id);
}