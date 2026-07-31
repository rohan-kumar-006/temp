using System.Diagnostics;
using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.DTOs.Products.Common;
using InventoryManagement.API.Helpers.Implementations;
using InventoryManagement.API.Models;
using InventoryManagement.API.Repositories.Implementations;
using InventoryManagement.API.Repositories.Interfaces;
using InventoryManagement.API.Services.Interfaces;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace InventoryManagement.API.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFileService _fileService;

    public ProductService(IProductRepository productRepository, IUserRepository userRepository, IFileService fileService)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _fileService = fileService;
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto request, int createdByUserId)
    {
        var existingProduct = await _productRepository.GetBySkuAsync(request.SKU);

        if (existingProduct != null)
        {
            throw new Exception("SKU already exists.");
        }

        var user = await _userRepository.GetByIdAsync(createdByUserId);

        if (user == null)
        {
            throw new KeyNotFoundException(
                "User not found."
            );
        }

        var imagePath = await _fileService.SaveImageAsync(request.Image);

        var product = new Product
        {
            Name = request.Name,
            SKU = request.SKU,
            Description = request.Description,
            Price = request.Price,
            Quantity = request.InitialQuantity,
            ReorderLevel = request.ReorderLevel,
            ImageUrl = imagePath,
            CreatedByUserId = createdByUserId,
            CreatedByUser = user,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        // ProductMapper.e
        // product.CreatedByUser = user;
        return ProductMapper.MapToProductDto(product);
    }

    public async Task<PagedResult<ProductDto>> GetProductsAsync(ProductQueryParameters parameters)
    {
        var result = await _productRepository.GetProductsAsync(parameters);
        var products = result.Items.Select(ProductMapper.MapToProductDto)
                    .ToList();

        return new PagedResult<ProductDto>
        {
            Items = products,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };
    }
}