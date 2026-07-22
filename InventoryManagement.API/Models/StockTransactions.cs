using System.ComponentModel.DataAnnotations;
using InventoryManagement.API.Enums;

namespace InventoryManagement.API.Models;

public class StockTransaction
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public Product? Product { get; set; }

    public int UserId { get; set; }

    public User? User { get; set; }

    public TransactionType Type { get; set; }

    [Range(0, int.MaxValue)]
    public int Quantity { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public DateTime CreatedAt { get; set; }
}