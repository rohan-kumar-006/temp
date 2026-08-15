using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.DTOs.Products.Common;
using InventoryManagement.API.Helpers.Implementations;
using InventoryManagement.API.Models;
using InventoryManagement.API.Repositories.Interfaces;
using InventoryManagement.API.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace InventoryManagement.API.Services.Implementations;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IUserRepository _userRepository;
    private readonly IFileService _fileService;
    private readonly ILogger<ProductService> _logger;
    private readonly IMemoryCache _cache;

    public ProductService(IProductRepository productRepository, IUserRepository userRepository,
     IFileService fileService, ILogger<ProductService> logger,IMemoryCache cache)
    {
        _productRepository = productRepository;
        _userRepository = userRepository;
        _fileService = fileService;
        _logger = logger;
        _cache = cache;
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
        _cache.Remove("admin-dashboard");
        _logger.LogInformation(
                "Product created. Id: {ProductId}, Name: {ProductName}, CreatedBy: {UserId}",
                product.Id, product.Name, createdByUserId
        );
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
    public async Task<ProductDto> UpdateProductAsync(int id, UpdateProductDto request)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        var existingProduct = await _productRepository.GetBySkuAsync(request.SKU);

        if (existingProduct != null && existingProduct.Id != id)
        {
            throw new ArgumentException("SKU already exists.");
        }

        product.Name = request.Name;

        product.SKU = request.SKU;

        product.Description = request.Description;

        product.Price = request.Price;

        product.ReorderLevel = request.ReorderLevel;

        product.UpdatedAt = DateTime.UtcNow;
        if (request.Image != null)
        {
            product.ImageUrl =
                await _fileService.SaveImageAsync(request.Image);
        }
        await _productRepository.SaveChangesAsync();
        _cache.Remove("admin-dashboard");
        _logger.LogInformation( "Product updated. Id: {ProductId}, Name: {ProductName}", product.Id,product.Name);
        
        return ProductMapper.MapToProductDto(product);
    }
    public async Task DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product == null)
        {
            throw new KeyNotFoundException("Product not found.");
        }
        var hasTransactions =
            await _productRepository.HasTransactionsAsync(id);

        if (hasTransactions)
        {
            throw new InvalidOperationException(
                "Cannot delete a product that has stock transactions."
            );
        }

        _logger.LogInformation("Product deleted. Id: {ProductId}, Name: {ProductName}", product.Id,product.Name);
        await _productRepository.DeleteAsync(product);

        await _productRepository.SaveChangesAsync();
        _cache.Remove("admin-dashboard");
    }
}