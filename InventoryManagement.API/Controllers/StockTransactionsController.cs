using InventoryManagement.API.Common;
using InventoryManagement.API.DTOs.StockTransactions;
using InventoryManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StockTransactionsController : ControllerBase
{
    private readonly IStockTransactionService _stockService;

    public StockTransactionsController(IStockTransactionService stockService)
    {
        _stockService = stockService;
    }   
 
    [Authorize(Roles ="Staff,Admin")]
    [HttpPost]
    public async Task<ActionResult<ApiResponse<StockTransactionResponseDto>>> CreateTransaction(CreateStockTransactionDto request)
    {
        var response = await _stockService.CreateTransactionAsync(request);
        return Ok(

        new ApiResponse<StockTransactionResponseDto>(
            true,
            "Stock transaction recorded successfully.",
            response
        )
    );
    }
}