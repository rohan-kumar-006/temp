using System.Security.Claims;
using InventoryManagement.API.Common;
using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.DTOs.Products.Common;
using InventoryManagement.API.Services.Implementations;
using InventoryManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/products")]
[Authorize(Roles = "Admin")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService)
    {
        _productService = productService;
    }


    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductDto>>> CreateProduct([FromForm] CreateProductDto request)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var product = await _productService.CreateProductAsync(request, userId);

        return Ok(new ApiResponse<ProductDto>(
            true,
            "Product added successfully",
            product
        ));
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<PagedResult<ProductDto>>>> GetProducts([FromQuery] ProductQueryParameters parameters)
    {
        var products = await _productService.GetProductsAsync(parameters);
        return new ApiResponse<PagedResult<ProductDto>>
        (
          true,
          "Products Sent Successfully",
            products
        );
    }
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateProduct(int id, [FromForm] UpdateProductDto request)
    {
        var product = await _productService.UpdateProductAsync(id, request);
        return Ok(
            new ApiResponse<ProductDto>(
                true,
                "Product Updated Successfully",
                product
            )
        );
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult<ApiResponse<object>>> DeleteProduct(int id)
    {
        await _productService.DeleteProductAsync(id);

        return Ok(
            new ApiResponse<object>(
                true,
                "Product deleted successfully.",
                null
            )
        );
    }
}