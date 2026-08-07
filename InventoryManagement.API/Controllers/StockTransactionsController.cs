using InventoryManagement.API.Common;
using InventoryManagement.API.DTOs.Products;
using InventoryManagement.API.DTOs.StockTransactions;
using InventoryManagement.API.DTOs.TransactionHistory;
using InventoryManagement.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles ="Staff,Admin")]

public class StockTransactionsController : ControllerBase
{
    private readonly IStockTransactionService _stockService;

    public StockTransactionsController(IStockTransactionService stockService)
    {
        _stockService = stockService;
    }   
 
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

    [HttpGet]
    public async Task<IActionResult> GetTransactionHistory([FromQuery] TransactionHistoryQueryParameters parameters)
    {
        var result=await _stockService.GetTransactionHistoryAsync(parameters);

        return Ok(new ApiResponse<PagedResult<TransactionHistoryDto>>(
            true,
            "Transaction History Retrived Successfully",
            result
        ));
    }
}