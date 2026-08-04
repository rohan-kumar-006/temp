using System.Security.Claims;
using InventoryManagement.API.DTOs.StockTransactions;
using InventoryManagement.API.Helpers.Implementations;
using InventoryManagement.API.Models;
using InventoryManagement.API.Repositories.Interfaces;
using InventoryManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Services.Implementations;

public class StockTransactionService : IStockTransactionService
{
    private readonly IProductRepository _productRepository;
    private readonly IStockTransactionRepository _stockRepository;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public StockTransactionService(IProductRepository productRepository, IStockTransactionRepository stockRepository,
    IHttpContextAccessor httpContextAccessor)
    {
        _productRepository = productRepository;
        _stockRepository = stockRepository;
        _httpContextAccessor = httpContextAccessor;
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
        
        return new StockTransactionResponseDto{
            Product=ProductMapper.MapToProductDto(product)
        };
    }
}