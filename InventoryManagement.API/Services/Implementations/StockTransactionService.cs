using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.DTOs.StockTransactions;
using InventoryManagement.API.DTOs.TransactionHistory;
using InventoryManagement.API.Helpers.Implementations;
using InventoryManagement.API.Models;
using InventoryManagement.API.Repositories.Interfaces;
using InventoryManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Claims;

namespace InventoryManagement.API.Services.Implementations;

public class StockTransactionService : IStockTransactionService
{
    private readonly IProductRepository _productRepository;
    private readonly IStockTransactionRepository _stockRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<StockTransactionService> _logger;
    private readonly IMemoryCache _cache;
    public StockTransactionService(IProductRepository productRepository, IStockTransactionRepository stockRepository,
    IHttpContextAccessor httpContextAccessor, ILogger<StockTransactionService> logger, IMemoryCache cache)
    {
        _productRepository = productRepository;
        _stockRepository = stockRepository;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _cache = cache;
    }

    public async Task<StockTransactionResponseDto> CreateTransactionAsync([FromBody] CreateStockTransactionDto request)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null)
        {
            throw new KeyNotFoundException(
                "Product not found."
            );
        }
        var userId = int.Parse(
            _httpContextAccessor
            .HttpContext!
            .User
            .FindFirst(ClaimTypes.NameIdentifier)!
            .Value
        );
        switch (request.Type)
        {
            case Enums.TransactionType.In:
                product.Quantity += request.Quantity;
                break;
            case Enums.TransactionType.Out:
                if (product.Quantity < request.Quantity)
                {
                    throw new ArgumentException(
                        "Insufficient stock."
                    );
                }
                product.Quantity -= request.Quantity;
                break;
        }
        product.UpdatedAt = DateTime.UtcNow;
        var transaction = new StockTransaction
        {
            ProductId = product.Id,
            UserId = userId,
            Type = request.Type,
            Quantity = request.Quantity,
            Remarks = request.Remarks,
            CreatedAt = DateTime.UtcNow
        };
        await _stockRepository.AddAsync(transaction);
        await _productRepository.SaveChangesAsync();
        _cache.Remove("admin-dashboard");
        _logger.LogInformation(
            "Stock transaction created. ProductId: {ProductId}, ProductName: {ProductName}, Type: {Type}, Quantity: {Quantity}, UserId: {UserId}",
            product.Id,
            product.Name,
            request.Type,
            request.Quantity,
            userId
        );

        return new StockTransactionResponseDto
        {
            Product = ProductMapper.MapToProductDto(product)
        };
    }

    public async Task<PagedResult<TransactionHistoryDto>> GetTransactionHistoryAsync(TransactionHistoryQueryParameters parameters)
    {
        var result = await _stockRepository.GetTransactionHistoryAsync(parameters);

        return new PagedResult<TransactionHistoryDto>
        {
            Items = result.Items.Select(t => new TransactionHistoryDto
            {
                Id = t.Id,
                ProductName = t.Product!.Name,
                SKU = t.Product.SKU,
                Type = t.Type,
                Quantity = t.Quantity,
                Remarks = t.Remarks,
                PerformedBy = t.User!.FullName,
                CreatedAt = DateTime.SpecifyKind(
                    t.CreatedAt,
                    DateTimeKind.Utc
                )
            }).ToList(),

            Page = result.Page,
            PageSize = result.PageSize,
            TotalItems = result.TotalItems,
            TotalPages = result.TotalPages
        };
    }
}